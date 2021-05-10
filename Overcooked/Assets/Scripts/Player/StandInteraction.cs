using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandInteraction : MonoBehaviour
{
    public float castDistance = 1f;
    private Collider m_Collider;
    private GameObject lookingAtStand;
    private GameObject selectedObject;
    public bool carryingObject;
    private Animator animator;
    private DefaultStandBehaviour standScript;



    void Start(){

        animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider>();
        lookingAtStand = null;
        selectedObject = null;
        carryingObject = false;
    }

    // Selects and highlights the stand where is looking at. Returns true if looking at a stand.
    private bool selectStand(){  
        RaycastHit hit;
        // Look at Cube Stand Logic:
        int selectableMask = 1 << 3;
        Vector3 startRaycast = transform.position;
        startRaycast.y += m_Collider.bounds.center.y / 4;
        if (Physics.Raycast(startRaycast, transform.TransformDirection (Vector3.forward), out hit, castDistance, selectableMask)) 
        {
            if(lookingAtStand != hit.transform.gameObject){ // If changed objects or looked at new object
                if(lookingAtStand != null) { // If changed object, deselect previous one
                    standScript.UnHighlight();
                }
                
                // And select new one:
                lookingAtStand = hit.transform.gameObject;
                standScript = lookingAtStand.GetComponent<DefaultStandBehaviour>();
                //Outline the object:
                standScript.Highlight();
            }
        } else if (lookingAtStand != null) { //Deselect object
            standScript.UnHighlight();
            lookingAtStand = null;
        }

        return lookingAtStand != null;
    }

    //Interactions depend of the stand's type, if the stand has an object on top, if the player carrying an object and depending on the button clicked. 
    void interactWithStand(){
        // Pick/Drop Logic
        if(Input.GetKeyUp(KeyCode.Space)){ // Pick or drop object with spacebar
            if(!carryingObject && standScript.hasItemOnTop){  // Pick up object
                selectedObject = standScript.GrabItem();
                carryingObject = true;
                animator.SetBool("isCarrying", true);
            }
            else if(carryingObject && !standScript.hasItemOnTop){  // Place object
                standScript.PlaceItem(selectedObject);
                selectedObject = null;
                carryingObject = false;
                animator.SetBool("isCarrying", false);
            }
        }
    }

    void Update()
    {
        if(selectStand()){ // If we are looking at a stand, interact with it
            interactWithStand();
        } else if(Input.GetKeyUp(KeyCode.Space) && carryingObject){
            selectedObject.GetComponent<PickUpObject>().Drop();
            selectedObject = null;
            carryingObject = false;
            animator.SetBool("isCarrying", false);
        }
    }
}
