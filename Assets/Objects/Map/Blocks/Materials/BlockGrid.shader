Shader "Custom/BlockGrid"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _LineColor ("Line Color", Color) = (0,0,0,1)
        _LineWidth ("Line Width", Range(0, 0.5)) = 0.02
        _GridSize ("Grid Size", Float) = 1.0
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            fixed4 _BaseColor;
            fixed4 _LineColor;
            float _LineWidth;
            float _GridSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 absNormal = abs(i.worldNormal);
                float2 grid_coords;

                if (absNormal.y > absNormal.x && absNormal.y > absNormal.z)
                {
                    grid_coords = i.worldPos.xz;
                }
                else if (absNormal.x > absNormal.y && absNormal.x > absNormal.z)
                {
                    grid_coords = i.worldPos.yz;
                }
                else
                {
                    grid_coords = i.worldPos.xy;
                }

                grid_coords /= _GridSize;

                float line_u = abs(frac(grid_coords.x) - 0.5);
                float line_v = abs(frac(grid_coords.y) - 0.5);

                if (line_u < _LineWidth * 0.5 || line_v < _LineWidth * 0.5)
                {
                    return _LineColor;
                }

                return _BaseColor;
            }
            ENDCG
        }
    }
}