Shader "Custom/BlockEdge"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _LineColor ("Line Color", Color) = (0,0,0,1)
        _LineWidth ("Line Width", Range(0,0.1)) = 0.02
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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _BaseColor;
            fixed4 _LineColor;
            float _LineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float edge = step(uv.x, _LineWidth) + step(1.0 - uv.x, _LineWidth)
                           + step(uv.y, _LineWidth) + step(1.0 - uv.y, _LineWidth);

                if (edge > 0)
                    return _LineColor;

                return _BaseColor;
            }
            ENDCG
        }
    }
}
