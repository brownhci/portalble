package edu.brown.cs.portalble.wrapper;

import android.content.Context;
import android.graphics.Bitmap;
import android.util.Log;

public class BmpProducer extends Thread {

    public static final String TAG = "Unity BmpProducer";

    private CustomFrameAvailableListner customFrameAvailableListner;

    public boolean onTestAndroid = false;
    private Bitmap bmp;
    private boolean release;

    public BmpProducer(Context context) {
        start();
    }

    public void loadBmp(Bitmap bmp) {
        this.bmp = bmp;
        release = false;
        Log.d(TAG, "loadBmp");
    }

    public void setCustomFrameAvailableListner(CustomFrameAvailableListner customFrameAvailableListner) {
        this.customFrameAvailableListner = customFrameAvailableListner;
    }

    @Override
    public void run() {
        super.run();
        while ((true)) {
            if (bmp == null || customFrameAvailableListner == null) continue;

            if(!onTestAndroid && release) continue;
            Log.d(TAG, "Writing frame");
            customFrameAvailableListner.onFrame(bmp);
            if(!onTestAndroid) release = true;

            try {
                Thread.sleep(10);
            } catch (Exception e) {
                Log.d(TAG, e.toString());
            }
        }
    }
}
