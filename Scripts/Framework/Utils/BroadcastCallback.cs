using Eremite;
using System;
using UnityEngine;

namespace Forwindz.Framework.Utils
{

    public class BroadcastCallback : IBroadcaster
    {
        public Action focusAction;
        public Action dismissAction;

        public BroadcastCallback(Action forcusAction, Action dismissAction)
        {
            this.focusAction = forcusAction;
            this.dismissAction = dismissAction;
        }

        public BroadcastCallback(Action forcusAction)
        {
            this.focusAction = forcusAction;
            this.dismissAction = () => { };
        }

        public void Dismiss()
        {
            dismissAction();
        }

        public void Focus()
        {
            focusAction();
        }
    }

    public class BroadcastCallbackTranslateCameraToPos : IBroadcaster
    {
        public Vector2Int field;

        public BroadcastCallbackTranslateCameraToPos(Vector2Int field)
        {
            this.field = field;
        }

        public void Dismiss()
        {
        }

        public void Focus()
        {
            SO.GameBlackboardService.OnFocusRequested.OnNext(field);
        }
    }
}
