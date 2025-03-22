Shader "Custom/2DOutlineFlicker"
{
    Properties
    {
        _MainTex ("Tile Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineThickness ("Outline Thickness", Range(0, 0.01)) = 0.002
        _FlickerSpeed ("Flicker Speed", Range(0,10)) = 2
        _FlickerIntensity ("Flicker Intensity", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        Pass
        {
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _OutlineColor;
            float  _OutlineThickness;
            float  _FlickerSpeed;
            float  _FlickerIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 读取当前像素
                float4 c = tex2D(_MainTex, i.uv);
                float alphaCenter = c.a;

                // 采样上下左右四点
                float2 offset = float2(_OutlineThickness, 0);
                float alphaLeft  = tex2D(_MainTex, i.uv - offset).a;
                float alphaRight = tex2D(_MainTex, i.uv + offset).a;
                float alphaUp    = tex2D(_MainTex, i.uv + float2(0,_OutlineThickness)).a;
                float alphaDown  = tex2D(_MainTex, i.uv - float2(0,_OutlineThickness)).a;

                // 判断是否边缘
                float threshold = 0.05;
                float isEdge = 0.0;
                if (abs(alphaLeft  - alphaCenter) > threshold ||
                    abs(alphaRight - alphaCenter) > threshold ||
                    abs(alphaUp    - alphaCenter) > threshold ||
                    abs(alphaDown  - alphaCenter) > threshold)
                {
                    isEdge = 1.0;
                }

                // 闪烁
                float flicker = sin(_Time.y * _FlickerSpeed);
                flicker = 1.0 + flicker * _FlickerIntensity; // 范围 [1 -I, 1 + I]

                // 计算描边颜色
                float4 edgeColor = _OutlineColor * flicker;

                // 把边缘像素与原颜色插值
                float4 finalColor = lerp(c, edgeColor, isEdge);
                finalColor.a = c.a;  // 保持原本的 Alpha
                return finalColor;
            }
            ENDCG
        }
    }
}
