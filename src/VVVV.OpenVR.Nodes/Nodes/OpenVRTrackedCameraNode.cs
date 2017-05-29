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
using SlimDX.Direct3D11;

using FeralTic.DX11.Resources;
using FeralTic.DX11;

using VVVV.DX11;

namespace VVVV.Nodes.ValveOpenVR
{
    [PluginInfo(Name = "TrackedCamera", Category = "OpenVR", AutoEvaluate = true, Tags = "vr, video, htc, vive, oculus, rift", Author = "tonfilm")]
    public class VOpenVRTrackedCameraNode : OpenVRConsumerBaseNode, IPluginEvaluate, IDX11ResourceHost
    {
        [Input("Corrected Texture", IsSingle = true, DefaultBoolean = true)]
        ISpread<bool> FCorrectedTextureIn;

        [Output("Texture Out", IsSingle = true)]
        protected Pin<DX11Resource<DX11Texture2D>> FTextureOutput;

        [Output("Is Valid")]
        protected ISpread<bool> FValid;

        OpenVRTrackedCamera.VideoStreamTexture FTexture;
        Texture2D _texture;
        protected bool FInvalidate;


        public override void Evaluate(int SpreadMax, CVRSystem system)
        {
            FTexture = OpenVRTrackedCamera.Source(FCorrectedTextureIn[0]);

            if (system != null)
            {
                this.FInvalidate = true;
            }
            else return;

            if (this.FTextureOutput[0] == null) { this.FTextureOutput[0] = new DX11Resource<DX11Texture2D>(); }            
        }

        public void Update(DX11RenderContext context)
        {
            if (this.FInvalidate)
            {
                if (!this.FTextureOutput[0].Contains(context))
                {
                    {
                        this.FTextureOutput[0].Dispose(context);
                    }

                    try     // copy to Output Pin
                    {
                        ShaderResourceView srv = new ShaderResourceView(context.Device, FTexture.texture);
                        this.FTextureOutput[0][context] = DX11Texture2D.FromTextureAndSRV(context, FTexture.texture, srv);
                        this.FValid[0] = true;
                    }
                    catch (Exception)
                    {
                        this.FValid[0] = false;
                    }
                }
                this.FInvalidate = false;
            }
        }

        public void Destroy(DX11RenderContext context, bool force)
        {
            for (int i = 0; i < FTextureOutput.SliceCount; i++)
                this.FTextureOutput[i].Dispose(context);
        }

    }
}
