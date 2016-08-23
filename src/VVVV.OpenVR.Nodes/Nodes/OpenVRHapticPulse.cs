using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils;
using Valve.VR;
using VVVV.Utils.VMath;
using SlimDX;
using System.Threading;
using System.Runtime.InteropServices;

namespace VVVV.Nodes.ValveOpenVR
{
    class OpenVRHapticPulse
    {
        [PluginInfo(Name = "HapticPulse", Category = "OpenVR", Tags = "vr, htc, vive, oculus, rift", Author = "sebl, tonfilm, herbst")]
        public class ValveOpenVRHapticPulseNode : IPluginEvaluate
        {
            [Input("Controller")]
            IDiffSpread<OpenVRController.Device> FControllerIn;

            [Input("HapticFeedback Enabled", IsBang = true)]
            IDiffSpread<bool> FInHapticEnabled;

            [Input("HapticFeedback Axis", IsBang = true)]
            IDiffSpread<EVRButtonId> FInHapticAxis;

            [Input("HapticFeedback Duration", DefaultValue = 500, MinValue = 5)]
            IDiffSpread<int> FInHapticDuration;

            [Output("Status")]
            ISpread<string> FOutStatus;

            public void Evaluate(int SpreadMax)
            {
                var system = OpenVRManager.System;
                if (FControllerIn != null && system != null)
                {
                    FOutStatus.SliceCount = SpreadMax;

                    for (int i = 0; i < SpreadMax; i++)
                    {
                        FOutStatus[i] = "Ok";

                        var controller = FControllerIn[i];
                        if (controller == null) continue; else FOutStatus[i] = "No Controller";

                        if (FInHapticEnabled[i])
                        {
                            // see: https://github.com/ValveSoftware/openvr/wiki/IVRSystem::TriggerHapticPulse
                            controller.TriggerHapticPulse((ushort)FInHapticDuration[i], FInHapticAxis[i]);
                        }
                    }
                }
                else
                {
                    FOutStatus.SliceCount = 1;
                    FOutStatus[0] = "No Controller or System";
                }
            }
        }
    }
}
