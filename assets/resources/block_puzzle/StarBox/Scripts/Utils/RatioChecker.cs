
using UnityEngine;
using UnityEngine.UI;

public class RatioChecker : MonoBehaviour {

    public CanvasScaler scaler;
   

    private void Awake() {
        CheckScalerOnEditor();
    }

    private void CheckScalerOnEditor() {
        Debug.Log("Scaler Canvas On Editor");
        if (Global.CAM_ASPECT >= 0.5f && Global.CAM_ASPECT < 0.5625f) {
            //Ratio 9:18
            scaler.matchWidthOrHeight = 0;
        } else if (Global.CAM_ASPECT >= 0.42f && Global.CAM_ASPECT < 0.5f) {
            //Iphone X - hoặc có tai thỏ
            // support to sonny xz4
            scaler.matchWidthOrHeight = 0;
        } else {
            //Ratio 9:16
            //Ratio 4:3 or less
            scaler.matchWidthOrHeight = 1;
        }
    }
}
