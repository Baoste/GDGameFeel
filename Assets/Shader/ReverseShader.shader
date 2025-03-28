Shader "Custom/ReverseShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed gray = dot(col.rgb, fixed3(0.299, 0.587, 0.114));
                fixed3 grayscaleColor = fixed3(gray, gray, gray);
                //float factor = _ElapsedTime * 5;
                fixed4 finalColor = col;
                //finalColor.rgb = lerp(col.rgb, grayscaleColor, factor);
                finalColor.rgb = grayscaleColor;

                return finalColor;
            }
            ENDCG
        }
    }
}
