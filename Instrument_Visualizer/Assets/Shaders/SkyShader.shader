Shader "Custom/BasicFragment"
{
    Properties
    {
        _ColorAverage("Color Average", Color) = (1, 0, 0, 0)
        _OverallDim("Overall Shader Dim", float) = 10
        _Dir("Direction", Vector) = (0.0, 1.0, 0.0)
        _Frequency ("Frequency", Float) = 5
        _Speed ("Speed", Float) = 1
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
            float4 _ColorAverage;
            float _Frequency;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float tA = length(i.uv - float2(_Dir.x, _Dir.y)) * _Frequency + _Time.y * _Speed;
                float valA = sin(tA) * 0.5 + 0.5;
                
                return float4(valA * float3(1, 1, 1), 1) * _ColorAverage / _OverallDim;
            }
            ENDCG
        }
    }
}
