using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
 
public class Movment : NetworkBehaviour
    { 
        public float Speed = 15f;
        public FixedJoystick fixedJoystick;
        public Animator anim;
        private void Update() {
            float inputX = Input.GetAxis("Horizontal")+ fixedJoystick.Horizontal;
            float inputY = Input.GetAxis("Vertical") + fixedJoystick.Vertical;
            
            transform.Translate(inputX * Speed * Time.deltaTime, inputY* Speed  * Time.deltaTime, 0f);
            
            anim.SetFloat("Horizontal",inputX);
            anim.SetFloat("Vertical",inputY);

            Vector3 characterScale = transform.localScale;
            if(inputX < 0)
            {
                characterScale.x = -1;
            }
            if(inputX > 0)
            {
                characterScale.x = 1;
            }
            transform.localScale = characterScale;
        }
    }