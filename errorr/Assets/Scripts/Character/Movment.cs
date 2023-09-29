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
            transform.Translate((Input.GetAxis("Horizontal")+ fixedJoystick.Horizontal) * Speed * Time.deltaTime, (Input.GetAxis("Vertical") + fixedJoystick.Vertical)* Speed  * Time.deltaTime, 0f);
            
            anim.SetFloat("Horizontal",(Input.GetAxis("Horizontal")+ fixedJoystick.Horizontal));
            anim.SetFloat("Vertical",(Input.GetAxis("Vertical") + fixedJoystick.Vertical));

            Vector3 characterScale = transform.localScale;
            if(Input.GetAxis("Horizontal") < 0)
        {
            characterScale.x = -1;
        }
        if(Input.GetAxis("Horizontal") > 0)
        {
            characterScale.x = 1;
        }
        transform.localScale = characterScale;
        }
    }