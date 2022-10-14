using System;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using HarmonyLib;
using VRC.Networking;
using VRC.Udon;
using VRC;
using VRC.SDKBase;
using UnityEngine;


namespace qfly
{
    public class Main : MelonMod
    {
		public static bool HasDoubleClicked(OVRInput.Button keyCode, float threshold)
		{
			bool flag = !OVRInput.GetDown(keyCode, OVRInput.Controller.Touch);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !Main.lastTime.ContainsKey(keyCode);
				if (flag2)
				{
					Main.lastTime.Add(keyCode, Time.time);
				}
				bool flag3 = Time.time - Main.lastTime[keyCode] <= threshold;
				if (flag3)
				{
					Main.lastTime[keyCode] = threshold * 2f;
					result = true;
				}
				else
				{
					Main.lastTime[keyCode] = Time.time;
					result = false;
				}
			}
			return result;
			
		}
		private Transform camera()
        {
			return VRCPlayer.field_Internal_Static_VRCPlayer_0.transform;
		}
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("I loaded :D");
        }
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if(buildIndex == -1)
            {
                this._originalgravity = Physics.gravity;
                this.loaded = true;
            }
        }
        public override void OnUpdate()
        {
			Player.prop_Player_0.gameObject.GetComponent<CharacterController>().enabled = !this.flytoggle;
			if (Main.HasDoubleClicked(OVRInput.Button.One, 0.25f))
            {
				this.flytoggle = !this.flytoggle;
            }
			
			if (!this.flytoggle)
			{
				if (this.loaded)
				{
					Physics.gravity = this._originalgravity;
				}
			}
			else if (this.flytoggle && !(Physics.gravity == Vector3.zero))
			{
				this._originalgravity = Physics.gravity;
				Physics.gravity = Vector3.zero;
				return;
			}
			if (this.flytoggle && !(Player.prop_Player_0 == null))
			{
				float num = Input.GetKey(KeyCode.LeftShift) ? (Time.deltaTime * 15f) : (Time.deltaTime * 10f);
				if (Player.prop_Player_0.field_Private_VRCPlayerApi_0.IsUserInVR())
				{
					if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") < -0.5f)
					{
						Player.prop_Player_0.transform.position -= this.camera().up * num;
					}
					if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") > 0.5f)
					{
						Player.prop_Player_0.transform.position += this.camera().up * num;
					}
					if (Input.GetAxis("Vertical") != 0f)
					{
						Player.prop_Player_0.transform.position += this.camera().forward * (num * Input.GetAxis("Vertical"));
					}
					if (Input.GetAxis("Horizontal") != 0f)
					{
						Player.prop_Player_0.transform.position += this.camera().transform.right * (num * Input.GetAxis("Horizontal"));
						return;
					}
				}
			}
		}
        private Vector3 _originalgravity;
        private bool loaded;
        private bool flytoggle;
		private OVRInput.Controller LastPressed;
		private static readonly Dictionary<OVRInput.Button, float> lastTime = new Dictionary<OVRInput.Button, float>();
	}

}
