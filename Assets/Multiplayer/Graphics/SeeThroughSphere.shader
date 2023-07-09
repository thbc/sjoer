Shader "Custom/SeeThroughSphere"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 0.5)
        _Radius("Radius", Range(0.1, 10)) = 1
    }

        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _Radius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 _Color;

            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate the distance from the center of the sphere
                float dist = length(i.vertex.xyz);

            // If the distance is greater than the radius, make the fragment transparent
            if (dist > _Radius)
            {
                discard;
            }

            // Apply the desired color with transparency
            fixed4 col = _Color;
            col.a *= smoothstep(_Radius, _Radius - 0.1, dist);
            return col;
        }
        ENDCG
    }
    }
}
