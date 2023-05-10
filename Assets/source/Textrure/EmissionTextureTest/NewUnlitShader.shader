Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		
        [HDR] _EmissionColor ("Emission Color", Color) = (1, 1, 1, 1)
        [HDR] _EmissionMap ("Emission", 2D) = "black" {}
        [HDR] _EmissionIntensity ("Emission Intensity", Range(0, 1000)) = 1
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
        sampler2D _EmissionMap;
        float4 _EmissionColor;
        float _EmissionIntensity;

        SamplerState g_samLinear
        {
            Filter = ANISOTROPIC;
            MaxAnisotropy = 8;
            AddressU = Wrap;
            AddressV = Wrap;
        };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb =  tex2D(_EmissionMap, i.uv).rgb * _EmissionColor * _EmissionIntensity;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
