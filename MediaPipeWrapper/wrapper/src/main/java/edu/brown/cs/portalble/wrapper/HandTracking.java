package edu.brown.cs.portalble.wrapper;


import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.util.Log;

import com.google.mediapipe.components.FrameProcessor;
import com.google.mediapipe.formats.proto.LandmarkProto;
import com.google.mediapipe.formats.proto.LandmarkProto.NormalizedLandmark;
import com.google.mediapipe.formats.proto.LandmarkProto.NormalizedLandmarkList;
import com.google.mediapipe.formats.proto.LocationDataProto;
import com.google.mediapipe.formats.proto.LocationDataProto.LocationData;
import com.google.mediapipe.framework.AndroidAssetUtil;
import com.google.mediapipe.framework.AndroidPacketCreator;
import com.google.mediapipe.framework.Packet;
import com.google.mediapipe.framework.PacketGetter;
import com.google.mediapipe.glutil.EglManager;

import java.io.ByteArrayInputStream;
import java.util.HashMap;
import java.util.List;
import java.util.Map;


public class HandTracking {

    private static final String TAG = "HandTracking";
    private static final String BINARY_GRAPH_NAME = "hand_tracking_mobile_gpu.binarypb";
    private static final String INPUT_VIDEO_STREAM_NAME = "input_video";
    private static final String OUTPUT_VIDEO_STREAM_NAME = "output_video";
    private static final String OUTPUT_LANDMARKS_STREAM_NAME = "hand_landmarks";
    private static final String OUTPUT_PALM_RECT_STREAM_NAME = "hand_rects_from_palm_detections";
    private static final String INPUT_NUM_HANDS_SIDE_PACKET_NAME = "num_hands";
    private static final int NUM_HANDS = 1;
    // Flips the camera-preview frames vertically before sending them into FrameProcessor to be
    // processed in a MediaPipe graph, and flips the processed frames back when they are displayed.
    // This is needed because OpenGL represents images assuming the image origin is at the bottom-left
    // corner, whereas MediaPipe in general assumes the image origin is at top-left.
    private static final boolean FLIP_FRAMES_VERTICALLY = true;

    static {
        // Load all native libraries needed by the app.
        System.loadLibrary("mediapipe_jni");
        System.loadLibrary("opencv_java3");
    }

    // Creates and manages an {@link EGLContext}.
    private EglManager eglManager;
    // Sends camera-preview frames into a MediaPipe graph for processing, and displays the processed
    // frames onto a {@link Surface}.
    private FrameProcessor processor;
    // Converts the GL_TEXTURE_EXTERNAL_OES texture from Android camera into a regular texture to be
    // consumed by {@link FrameProcessor} and the underlying MediaPipe graph.
    private MpExternalTextureConverter converter;
    private List<NormalizedLandmarkList> multiHandLandMarks;
    private List<LocationData.BoundingBox> palmRects;
    private NormalizedLandmark singleHandLandmarks;
    // ApplicationInfo for retrieving metadata defined in the manifest.
    private int defaultWithSize = 512;
    private Context context;
    private BmpProducer bitmapProducer;
    private Long lastTimeStamp;

    public HandTracking(Context context) {
        this.context = context;
        AndroidAssetUtil.initializeNativeAssetManager(context);

        eglManager = new EglManager(null);
        processor = new FrameProcessor(context,
                        eglManager.getNativeContext(),
                        BINARY_GRAPH_NAME,
                        INPUT_VIDEO_STREAM_NAME,
                        OUTPUT_VIDEO_STREAM_NAME);
        processor
                .getVideoSurfaceOutput()
                .setFlipY(FLIP_FRAMES_VERTICALLY);

        AndroidPacketCreator packetCreator = processor.getPacketCreator();
        Map<String, Packet> inputSidePackets = new HashMap<>();
        inputSidePackets.put(INPUT_NUM_HANDS_SIDE_PACKET_NAME, packetCreator.createInt32(NUM_HANDS));
        processor.setInputSidePackets(inputSidePackets);


        processor.addPacketCallback(OUTPUT_PALM_RECT_STREAM_NAME, (packet -> {
            try {
                Log.d(TAG, "Received palm rect packet.");
                List<LocationDataProto.LocationData.BoundingBox> palmRects =
                        PacketGetter.getProtoVector(packet, LocationDataProto.LocationData.BoundingBox.parser());
                Log.d(TAG, "palm rect size:" + palmRects.size());
                this.palmRects = palmRects;
                Log.d(TAG, "[TS:" + packet.getTimestamp() + "] Rect: " + getRectDebugString(this.palmRects.get(0)));
            } catch (Exception e) {
                Log.e(TAG, "Couldn't Exception received - " + e);
                return;
            }
        }));

        processor.addPacketCallback(
                OUTPUT_LANDMARKS_STREAM_NAME,
                (packet) -> {
                    try {
                        Log.d(TAG, "Received multi-hand landmarks packet");
//                        byte[] landmarksRaw = PacketGetter.getProtoBytes(packet);
//                        Log.d(TAG, "Get LandMarksRaw Mediapipe: " + landmarksRaw.length);
//                        LandmarkProto.NormalizedLandmarkList landmarks = LandmarkProto.NormalizedLandmarkList.parseFrom(landmarksRaw);
//                        Log.d(TAG, "Get LandMarks Mediapipe: " + landmarks.toString());
//                        Log.d(TAG, "[TS:"
//                                + packet.getTimestamp()
//                                + "] "
//                                + getLandmarksDebugString(landmarks));
                        List<NormalizedLandmarkList> multiHandLandmarks =
                                PacketGetter.getProtoVector(packet, NormalizedLandmarkList.parser());
                        Log.d(TAG, "Packet LandMarks Size: " + multiHandLandmarks.size());
                        this.lastTimeStamp = System.currentTimeMillis();
                        this.multiHandLandMarks = multiHandLandmarks;
                        Log.d(
                                TAG,
                                "[TS:"
                                        + packet.getTimestamp()
                                        + "] "
                                        + getMultiHandLandmarksDebugString(multiHandLandmarks));
                    } catch(Exception e){
                        Log.d(TAG, e.getMessage());
                    }
                });

        converter = new MpExternalTextureConverter(eglManager.getContext());
        converter.setFlipY(FLIP_FRAMES_VERTICALLY);
        converter.setConsumer(processor);

        bitmapProducer = new BmpProducer(context);
        bitmapProducer.setCustomFrameAvailableListner(converter);
    }

    public void setResolution(int resolution) {
        Log.d(TAG, "set resolution");
        this.defaultWithSize = resolution;
    }

    public float[] getLandmarks(long interval) {
        Log.d(TAG, "get landmarks...");
        if (this.lastTimeStamp == null || System.currentTimeMillis() - this.lastTimeStamp > interval){
            return null;
        }
        if (null == this.multiHandLandMarks || this.multiHandLandMarks.isEmpty()) {
            Log.d(TAG, "get landmarks fail <<<<");
            return null;
        }
        float[] landmarks = new float[63];
        for (NormalizedLandmarkList singlehandlandmarks : this.multiHandLandMarks) {
            for (int i = 0; i < 21; i++) {
                NormalizedLandmark landmark = singlehandlandmarks.getLandmarkList().get(i);
                landmarks[3 * i] = landmark.getX();
                landmarks[3 * i + 1] = landmark.getY();
                landmarks[3 * i + 2] = landmark.getZ();
            }
            break;
        }
        Log.d(TAG, landmarks.toString());
        Log.d(TAG, "get landmarks success >>>>");
        return landmarks;
    }

    public float[] getPalmRect() {
        Log.d(TAG, "get palm rect...");
        if (null == this.palmRects) {
            Log.d(TAG, "get palm rect fail <<<<");
            return null;
        }
        float[] palmRect = new float[5];
        palmRect[0] = this.palmRects.get(0).getWidth();
        palmRect[1] = this.palmRects.get(0).getHeight();
        palmRect[2] = this.palmRects.get(0).getXmin();
        palmRect[3] = this.palmRects.get(0).getYmin();
//        palmRect[4] = this.palmRect.getRotation();
        this.palmRects = null;
        Log.d(TAG, "get palm rect success >>>>");
        return palmRect;
    }

    public void setFrame(byte[] frameSource) {
        Log.d(TAG, "set frame...");
        //attack frame bit map to GPU
        ByteArrayInputStream ims = new ByteArrayInputStream(frameSource);
        Drawable d = Drawable.createFromStream(ims, null);
        Bitmap bitmap = ((BitmapDrawable) d).getBitmap();
        float ratio = ((float) bitmap.getHeight()) / ((float) bitmap.getWidth());
        bitmap = Bitmap.createScaledBitmap(bitmap, defaultWithSize, (int) (defaultWithSize * ratio), true);
//        Bitmap bmp = BitmapFactory.decodeResource(this.context.getResources(), R.drawable.img2);
//        bmp = Bitmap.createScaledBitmap(bmp, 480, 640, true);
//        Log.d(TAG, "unity frame substituted by fixed image");
        bitmapProducer.loadBmp(bitmap);
        Log.d(TAG, String.valueOf(frameSource.length));
        Log.d(TAG, "set frame success >>>>");
    }

    /*
    public void test() {
        Log.d(TAG, "test ... ");
        //attack default frame bit map to GPU
        Bitmap bitmap = BitmapFactory.decodeResource(this.context.getResources(), R.drawable.img2);
        bitmap = Bitmap.createScaledBitmap(bitmap, 480, 640, true);
        bitmapProducer.loadBmp(bitmap);
        Log.d(TAG, "test success >>>> ");
    }
    */

    public void dispose() {
        converter. close();
    }

    private String getMultiHandLandmarksDebugString(List<NormalizedLandmarkList> multiHandLandmarks) {
        if (multiHandLandmarks.isEmpty()) {
            return "No hand landmarks";
        }
        String multiHandLandmarksStr = "Number of hands detected: " + multiHandLandmarks.size() + "\n";
        int handIndex = 0;
        for (NormalizedLandmarkList landmarks : multiHandLandmarks) {
            multiHandLandmarksStr +=
                    "\t#Hand landmarks for hand[" + handIndex + "]: " + landmarks.getLandmarkCount() + "\n";
            int landmarkIndex = 0;
            for (NormalizedLandmark landmark : landmarks.getLandmarkList()) {
                multiHandLandmarksStr +=
                        "\t\tLandmark ["
                                + landmarkIndex
                                + "]: ("
                                + landmark.getX()
                                + ", "
                                + landmark.getY()
                                + ", "
                                + landmark.getZ()
                                + ")\n";
                ++landmarkIndex;
            }
            ++handIndex;
        }
        return multiHandLandmarksStr;
    }

    private static String getLandmarksDebugString(LandmarkProto.NormalizedLandmarkList landmarks) {
        int landmarkIndex = 0;
        String landmarksString = "";
        for (LandmarkProto.NormalizedLandmark landmark : landmarks.getLandmarkList()) {
            landmarksString += "\t\tLandmark[" + landmarkIndex + "]: (" + landmark.getX() + ", " + landmark.getY() + ", " + landmark.getZ() + ")\n";
            ++landmarkIndex;
        }
        return landmarksString;
    }

    private static String getRectDebugString(LocationData.BoundingBox rect) {
        String rectString = "x_center: " + rect.getXmin() + ", y_center: " + rect.getYmin() + ", height: " + rect.getHeight() + ", width: " + rect.getWidth();
        return rectString;
    }
}

