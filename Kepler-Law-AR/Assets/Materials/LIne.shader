Shader "Unlit/LIne"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _DashLength ("Dash Length", Float) = 0.2
        _GapLength ("Gap Length", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 prevVertex : TEXCOORD0; // Vertex sebelumnya
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 prevWorldPos : TEXCOORD1;
            };

            float4 _Color;
            float _DashLength;
            float _GapLength;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.prevWorldPos = mul(unity_ObjectToWorld, float4(v.prevVertex, 1)).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 direction = i.worldPos - i.prevWorldPos;
                float dist = length(direction);
                float total = _DashLength + _GapLength;
                float modDist = fmod(dist, total);
                clip(modDist - _DashLength); // Potong jika di bagian gap
                return _Color;
            }
            ENDCG
        }
    }
}
