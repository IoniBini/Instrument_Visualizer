Shader "Unlit/BasicFragment"
{
    Properties
    {
        _ColorBlend("Color Blend", Range(0, 1)) = 0
        _FromColor("From Color", Color) = (0, 0, 0, 1)
        _Emission1("From Color Emission", float) = 1
        _ToColor("To Color", Color) = (1,1,1,1)
        _Emission2("To Color Emission", float) = 1
        _NoInputBlend("No Input Blend", Range(0, 1)) = 0
        _OverallDim("Overall Shader Dim", Range(0, 1)) = 0
        [HideInInspector] _NoInputColor("No Input Color", Color) = (0, 0, 0, 0) //this is hidden because I always want no input to be without any colour, or black
        _Dir("Direction", Vector) = (0.0, 1.0, 0.0)
        _FrequencyA ("Frequency A", Float) = 5
        _SpeedA ("Speed A", Float) = 1
        _FrequencyB ("Frequency B", Float) = 5
        _SpeedB ("Speed B", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            Vector _Dir;
            float _OverallDim;
            float _Emission1;
            float _Emission2;
            float _ColorBlend;
            float _NoInputBlend;
            float4 _NoInputColor;
            float4 _FromColor;
            float4 _ToColor;
            float _FrequencyA;
            float _FrequencyB;
            float _SpeedA;
            float _SpeedB;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float tA = length(i.uv - float2(_Dir.x, _Dir.y)) * _FrequencyA + _Time.y * _SpeedA;
                float valA = sin(tA) * 0.5 + 0.5;
                
                float tB = length(i.uv - float2(_Dir.z, _Dir.w)) * _FrequencyB + _Time.y * _SpeedB;
                float valB = sin(tB) * 0.5 + 0.5;

                //fixed4 blend = lerp(valA, valB, _ColorBlend + sin(_Time.y));
                fixed4 blend = lerp(valA + _FromColor * _Emission1, valB + _ToColor * _Emission2, _ColorBlend);

                //fixed4 col = lerp(_FromColor * _Emission1, _ToColor * _Emission2, blend);
                fixed4 colVsInput = lerp(blend, _NoInputColor, _NoInputBlend + _OverallDim);
                
                return float4((valA + valB) * float3(1, 1, 1), 1) * colVsInput;
            }
            ENDCG
        }
    }
}
