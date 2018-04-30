using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {

    // Book
    [Header ("Book")]
    public GameObject bookObj;
    GameObject bookTransformObj;
    Animator bookAnim;
    public float bookAnimStartDelay;
    public float bookAnimStopTime;
    float currentBookTime = 0f;
    public Vector3 bookStopTranslate;
    public Vector3 bookOriginalTranslate;
    public Vector3 bookStopRotate;
    public Vector3 bookOriginalRotate;

    // MONOBEHAVIOR --------------------------------------------------
    private void Awake() {
        bookTransformObj = bookObj.transform.GetChild(0).gameObject;
        bookAnim = bookTransformObj.transform.GetChild(0).gameObject.GetComponent<Animator>();
        // Reset
        bookTransformObj.transform.position = bookOriginalTranslate;
        bookTransformObj.transform.localEulerAngles = bookOriginalRotate;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        currentBookTime += Time.deltaTime;
        if(currentBookTime >= bookAnimStartDelay) {
            // Rotate and Translate
            float _normalized = (currentBookTime - bookAnimStartDelay) / bookAnimStopTime;
            bookTransformObj.transform.position = Vector3.Lerp(bookOriginalTranslate, bookStopTranslate, _normalized);
            bookTransformObj.transform.localEulerAngles = Vector3.Lerp(bookOriginalRotate, bookStopRotate, _normalized);
            // Animate opening
            if (!bookAnim.GetBool("AnimOpen")) {
                bookAnim.SetBool("AnimOpen", true);
            }
        }
	}
}
