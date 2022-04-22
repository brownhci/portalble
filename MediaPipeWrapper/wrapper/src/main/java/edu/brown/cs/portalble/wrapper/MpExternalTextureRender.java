// Copyright 2019 The MediaPipe Authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

package edu.brown.cs.portalble.wrapper;

import android.graphics.Bitmap;
import android.graphics.SurfaceTexture;
import android.opengl.GLES11Ext;
import android.opengl.GLES20;
import android.opengl.GLUtils;
import android.util.Log;

import com.google.mediapipe.glutil.CommonShaders;
import com.google.mediapipe.glutil.ShaderUtil;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.FloatBuffer;
import java.nio.ShortBuffer;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.locks.ReentrantLock;

/**
 * Textures from {@link SurfaceTexture} are only supposed to be bound to target {@link
 * GLES11Ext#GL_TEXTURE_EXTERNAL_OES}, which is accessed using samplerExternalOES in the shader.
 * This means they cannot be used with a regular shader that expects a sampler2D. This class renders
 * the external texture to the current framebuffer. By binding the framebuffer to a texture, this
 * can be used to convert the input into a normal 2D texture.
 */
public class MpExternalTextureRender {

    public static Bitmap img;

    private static final FloatBuffer TEXTURE_VERTICES =
            ShaderUtil.floatBuffer(
                    0.0f, 0.0f, // bottom left
                    1.0f, 0.0f, // bottom right
                    0.0f, 1.0f, // top left
                    1.0f, 1.0f // top right
            );

    private static final FloatBuffer FLIPPED_TEXTURE_VERTICES =
            ShaderUtil.floatBuffer(
                    0.0f, 1.0f, // top left
                    1.0f, 1.0f, // top right
                    0.0f, 0.0f, // bottom left
                    1.0f, 0.0f // bottom right
            );

    static float mXYZCoords[] = {-1.0f, 1.0f, 0.0f, // top left
            -1.0f, -1.0f, 0.0f, // bottom left
            1.0f, -1.0f, 0.0f, // bottom right
            1.0f, 1.0f, 0.0f // top right
    };

    private static final String TAG = "ExternalTextureRend"; // Max length of a tag is 23.
    private static final int ATTRIB_POSITION = 1;
    private static final int ATTRIB_TEXTURE_COORDINATE = 2;

    private int program = 0;
    private int frameUniform;

    private int textureTransformUniform;
    private float[] textureTransformMatrix = new float[16];
    private boolean flipY;

    private FloatBuffer verticesBuffer;
    private FloatBuffer textureBuffer;

    int mTextureIds[] = new int[4];
    float[] mScaleMatrix = new float[16];

    private FloatBuffer mVertexBuffer;
    private FloatBuffer mTextureBuffer;
    private ShortBuffer mDrawListBuffer;

    boolean mVideoFitEnabled = true;
    boolean mVideoDisabled = false;

    // number of coordinates per vertex in this array
    static final int COORDS_PER_VERTEX = 1;
    static final int TEXTURECOORDS_PER_VERTEX = 4;

    static float mUVCoords[] = {0, 0, // top left
            0, 1, // bottom left
            1, 1, // bottom right
            1, 0}; // top right

    private short mVertexIndex[] = {0, 1, 2, 0, 2, 3}; // order to draw
    // vertices
    ReentrantLock mFrameLock = new ReentrantLock();

    private int mTextureWidth;
    private int mTextureHeight;
    private int mViewportWidth;
    private int mViewportHeight;

    public MpExternalTextureRender() {
        ByteBuffer bb = ByteBuffer.allocateDirect(mXYZCoords.length * 4);
        bb.order(ByteOrder.nativeOrder());
        mVertexBuffer = bb.asFloatBuffer();
        mVertexBuffer.put(mXYZCoords);
        mVertexBuffer.position(0);

        ByteBuffer tb = ByteBuffer.allocateDirect(mUVCoords.length * 4);
        tb.order(ByteOrder.nativeOrder());
        mTextureBuffer = tb.asFloatBuffer();
        mTextureBuffer.put(mUVCoords);
        mTextureBuffer.position(0);

        ByteBuffer dlb = ByteBuffer.allocateDirect(mVertexIndex.length * 2);
        dlb.order(ByteOrder.nativeOrder());
        mDrawListBuffer = dlb.asShortBuffer();

        mDrawListBuffer.put(mVertexIndex);
        mDrawListBuffer.position(0);
    }

    /** Call this to setup the shader program before rendering. */
    public void setup() {
        initializeBuffers();
        Map<String, Integer> attributeLocations = new HashMap<>();
        attributeLocations.put("position", ATTRIB_POSITION);
        attributeLocations.put("texture_coordinate", ATTRIB_TEXTURE_COORDINATE);
        program =
                ShaderUtil.createProgram(
                        VERTEX_SHADER,
                        fragmentShaderCode,
                        attributeLocations);
        frameUniform = GLES20.glGetUniformLocation(program, "video_frame");


        textureTransformUniform = GLES20.glGetUniformLocation(program, "texture_transform");
        ShaderUtil.checkGlError("glGetUniformLocation");





    }
    public static final FloatBuffer SQUARE_VERTICES =
            ShaderUtil.floatBuffer(
                    -1.0f, -1.0f,  // bottom left
                    1.0f, -1.0f,   // bottom right
                    -1.0f, 1.0f,   // top left
                    1.0f, 1.0f     // top right
            );
    /**
     * Vertices for a quad that fills the drawing area, but rotated 90 degrees.
     */
    public static final FloatBuffer ROTATED_SQUARE_VERTICES =
            ShaderUtil.floatBuffer(
                    -1.0f, 1.0f,   // top left
                    -1.0f, -1.0f,  // bottom left
                    1.0f, 1.0f,    // top right
                    1.0f, -1.0f    // bottom right
            );
    public static final String VERTEX_SHADER =
            "uniform mat4 texture_transform;\n"
                    + "attribute vec4 position;\n"
                    + "attribute mediump vec4 texture_coordinate;\n"
                    + "varying mediump vec2 sample_coordinate;\n"
                    + "\n"
                    + "void main() {\n"
                    + "  gl_Position = position;\n"
                    + "  sample_coordinate = (texture_transform * texture_coordinate).xy;\n"
                    + "}";

    private final String fragmentShaderCode =
            "#extension GL_OES_EGL_image_external : require\n"+
                    "varying mediump vec2 sample_coordinate;\n"
                    + "uniform sampler2D video_frame;\n"
                    //+"uniform sampler2D bg_frame;\n"
                    + "\n"
                    + "void main() {\n"
                    + "  gl_FragColor = texture2D(video_frame, sample_coordinate);\n"
                    // +"gl_FragColor=vec4(1.0,0.0,1.0,1.0);\n"
                    + "}";


    //Spesific Implimentation

    public static float[] vertices = {
            -1f, -1f,
            1f, -1f,
            -1f, 1f,
            1f, 1f
    };

    public static float[] textureVertices = {
            0f, 1f,
            1f, 1f,
            0f, 0f,
            1f, 0f
    };
    private void initializeBuffers() {

        ByteBuffer buff = ByteBuffer.allocateDirect(vertices.length * 4);
        buff.order(ByteOrder.nativeOrder());
        verticesBuffer = buff.asFloatBuffer();
        verticesBuffer.put(vertices);
        verticesBuffer.position(0);
        buff = ByteBuffer.allocateDirect(textureVertices.length * 4);
        buff.order(ByteOrder.nativeOrder());
        textureBuffer = buff.asFloatBuffer();
        textureBuffer.put(textureVertices);
        textureBuffer.position(0);
    }

    /**
     * Flips rendering output vertically, useful for conversion between coordinate systems with
     * top-left v.s. bottom-left origins. Effective in subsequent {@link #(SurfaceTexture)}
     * calls.
     */
    public void setFlipY(boolean flip) {
        flipY = flip;
    }

    /**
     * Renders the Bitmap to the framebuffer with optional vertical flip.
     *
     * <p>Before calling this, {@link #setup} must have been called.
     *
     * <p>NOTE: Calls {@link SurfaceTexture#updateTexImage()} on passed surface texture.
     */



    public void render(int textureName){
        GLES20.glClear(GLES20.GL_COLOR_BUFFER_BIT);
        GLES20.glActiveTexture(GLES20.GL_TEXTURE0);
        GLUtils.texImage2D(GLES20.GL_TEXTURE_2D, 0, frame
                , 0);
        GLES20.glTexParameteri(GLES20.GL_TEXTURE_2D, GLES20.GL_TEXTURE_MAG_FILTER,GLES20.GL_LINEAR);
        GLES20.glTexParameteri(GLES20.GL_TEXTURE_2D, GLES20.GL_TEXTURE_MIN_FILTER,GLES20.GL_LINEAR);
        GLES20.glTexParameteri(GLES20.GL_TEXTURE_2D, GLES20.GL_TEXTURE_WRAP_S, GLES20.GL_CLAMP_TO_EDGE);
        GLES20.glTexParameteri(GLES20.GL_TEXTURE_2D, GLES20.GL_TEXTURE_WRAP_T, GLES20.GL_CLAMP_TO_EDGE);
        GLES20.glUseProgram(program);
        GLES20.glUniform1i(frameUniform, 0);
        GLES20.glUniformMatrix4fv(textureTransformUniform, 1, false, textureTransformMatrix, 0);
        ShaderUtil.checkGlError("glUniformMatrix4fv");
        GLES20.glEnableVertexAttribArray(ATTRIB_POSITION);
        GLES20.glVertexAttribPointer(
                ATTRIB_POSITION, 2, GLES20.GL_FLOAT, false, 0, CommonShaders.SQUARE_VERTICES);
        GLES20.glEnableVertexAttribArray(ATTRIB_TEXTURE_COORDINATE);
        GLES20.glVertexAttribPointer(
                ATTRIB_TEXTURE_COORDINATE,
                2,
                GLES20.GL_FLOAT,
                false,
                0,
                flipY ? FLIPPED_TEXTURE_VERTICES : TEXTURE_VERTICES);
        ShaderUtil.checkGlError("program setup");
        GLES20.glDrawArrays(GLES20.GL_TRIANGLE_STRIP, 0, 4);
        GLES20.glBindTexture(GLES20.GL_TEXTURE_2D, textureName);
        ShaderUtil.checkGlError("glDrawArrays");
    }

    Bitmap frame;

    public void displayFrame(Bitmap bitmap)
    {
        Log.d("RENDER:", "Lock!!");
        mFrameLock.lock();
        mTextureWidth = bitmap.getWidth();
        mTextureHeight = bitmap.getHeight();
        this.frame = bitmap;
        Log.d("RENDER:", String.valueOf(bitmap.getByteCount())); //mylog
        mFrameLock.unlock();
        Log.d("RENDER:", "UnLock!!");
    }




    /**
     * Call this to delete the shader program.
     *
     * <p>This is only necessary if one wants to release the program while keeping the context around.
     */
    public void release() {
        GLES20.glDeleteProgram(program);
    }
}
