Shader "Unlit/DustCircle_DoubleSidedTrasparent"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Speed("Speed", Range(0,5)) = 1
        _TransparentHight("TransparentHight", Range(0,1000)) = 10
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off // ���������� ���������� (Culling)

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
                // ���������� ��� �������� �������� ��������
                fixed2 offset;
                // ���������� ��� �������� �������� �������� ��������
                fixed _Speed;
                fixed4 _Color;
                fixed _TransparentHight;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                // ������� ��� ���������� ������������ ��������
                float CalculateAlpha(fixed2 uv : TEXCOORD0)
                {
                    // ��������� ������������ ���������� ��������
                    fixed v = uv.y;
                    v = pow(v, 3);
                    // ����������� ���������� �������� ��� ��������� ������������
                    fixed threshold = _TransparentHight;

                    // ���������� ������������ �� ������ ������������ ����������
                    fixed alpha = smoothstep(threshold, 1.0, v);

                    return alpha;
                }

                // ������� ��� �������� ��������
                fixed4 MoveTexture(fixed2 uv : TEXCOORD0, fixed time : TIME) : SV_Target
                {
                    // ���������� �������� �������� �� ��� X �� ������ ������� � ��������
                    fixed offsetValue = time * _Speed;
                    fixed2 offsetUV = fixed2(offsetValue, 0);

                    // ��������� ���������� U �������� � ������ ��������
                    uv += offset + offsetUV;

                    // ��������� ����� ������� �� ��������
                    fixed4 color = tex2D(_MainTex, uv);

                    return color;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // ����� ������� MoveTexture � �������� ���������� ����������� UV � �������� �������
                    fixed4 col = MoveTexture(i.uv, _Time.y);
                    col.rgb = _Color.rgb;
                    col.a *= CalculateAlpha(i.uv);
                    return col;
                }
                ENDCG
            }
        }
}
