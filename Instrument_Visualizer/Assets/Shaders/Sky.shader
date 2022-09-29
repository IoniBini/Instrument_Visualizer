Shader "Custom/Sky"{
    //show values to edit in inspector
    Properties{
      _FromColor("From Color", Color) = (0, 0, 0, 1) //the base color
      _ToColor("To Color", Color) = (1,1,1,1) //the color to blend to
      _Emission1("Emission Intensity 1", float) = 1
      _Emission2("Emission Intensity 2", float) = 1
      _TimeMultiplier("Time Multiplier", float) = 1
    }

        SubShader{
        //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
        Tags{ "RenderType" = "Opaque" "Queue" = "Geometry"}

        Pass{
          CGPROGRAM

          //include useful shader functions

          //define vertex and fragment shader
          #pragma vertex vert
          #pragma fragment frag

          //the colors to blend between
          fixed4 _FromColor;
          fixed4 _ToColor;
          float _Emission1;
          float _Emission2;
          float _TimeMultiplier;

          //the object data that's put into the vertex shader
          struct appdata {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
          };

          //the data that's used to generate fragments and can be read by the fragment shader
          struct v2f {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
          };

          //the vertex shader
          v2f vert(appdata v) {
            v2f o;
            //convert the vertex positions from object space to clip space so they can be rendered
            o.position = UnityObjectToClipPos(v.vertex);
            //o.uv = v.uv;
            o.uv = sin(_Time.y * _TimeMultiplier);
            return o;
          }

          //the fragment shader
          fixed4 frag(v2f i) : SV_TARGET{
              float blend = i.uv.y;
            fixed4 col = lerp(_FromColor * _Emission1, _ToColor * _Emission2, blend);
            return col;
          }

          ENDCG
        }
    }
}