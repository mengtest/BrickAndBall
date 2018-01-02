﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State {
	using Component;
	using Utility;
	using StateData = Data.Normal;

	public class Normal : State {
		private StateData data;
		private Controller controller;
		private float clickTime;
		private float dragValue;
		private bool coolDown;
		private Timer timer;
		private int process;
		private Color originColor;
		private Color targetColor;
		private Vector3 draggingPos;

		public Normal (GameObject gameObject, StateData data) : base (gameObject, data) {
			this.data = data;
			this.controller = gameObject.GetComponent<Controller> ();
			this.dragValue = 0.5f;
			this.coolDown = false;
			this.timer = new Timer ();
		}

		public override void Update() {
			if (!this.coolDown) {
				return;
			}

			this.timer.Update (Time.fixedDeltaTime);
			this.controller.ColorLert (this.originColor, this.targetColor, this.timer.GetProcess ());

			if (!this.timer.IsRunning ()) {
				this.process += 1;

				if (this.process <= 2) {
					this.timer.Enter (0.3f);

					if (this.process == 1) {
						Sound.Play (this.data.clip);
						this.originColor = this.controller.originColor;
						this.targetColor = Color.white;
					} else {
						this.originColor = Color.white;
						this.targetColor = this.controller.originColor;
					}
				} else {
					this.coolDown = false;
					this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
				}
			}
		}

		public override void Enter() {
			this.timer.Enter (this.data.coolDownTime);
			this.process = 0;
			this.originColor = this.controller.targetColor;
			this.targetColor = this.controller.originColor;
		}

		public override void Exit() {
			this.coolDown = true;
		}

		public override void OnMouseDown () {
			if (this.coolDown || !this.controller.canControll) {
				return;
			}

			this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
		}

		public override void OnMouseDrag () {
			if (this.coolDown || !this.controller.canControll) {
				return;
			}

			Vector3 oldPos = this.draggingPos;
			this.draggingPos = ViceCamera.ScreenToWorldPoint (Input.mousePosition);
			Vector3 newPos = this.draggingPos;
			float delta = newPos.x - oldPos.x;

			if ((this.controller.direction > 0 && delta > this.dragValue) || (this.controller.direction < 0 && delta < -this.dragValue)) {
				this.statemgr.Play ("Elast");
			}
		}
	}
}

