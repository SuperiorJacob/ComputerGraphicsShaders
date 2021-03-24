Shader "PostProcssing/EdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Multiplier ("Edge Strength", Range(0.0, 10.0)) = 5.0
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

            float _Multiplier;

            float sobel(sampler2D tex, float2 uv)
            {
                float2 delta = float2(1.0f/_ScreenParams.x, 1.0f/_ScreenParams.y);
                float4 hr = float4(0, 0, 0, 0);
                float4 vt = float4(0, 0, 0, 0);

                hr += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) *  1.0;
                hr += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) * -1.0;
                hr += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) *  2.0;
                hr += tex2D(tex, (uv + float2( 1.0,  0.0) * delta)) * -2.0;
                hr += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) *  1.0;
                hr += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) * -1.0;

                hr += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) *  1.0;
                hr += tex2D(tex, (uv + float2( 0.0, -1.0) * delta)) *  2.0;
                hr += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) *  1.0;
                hr += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) * -1.0;
                hr += tex2D(tex, (uv + float2( 0.0,  1.0) * delta)) * -2.0;
                hr += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) * -1.0;

                return saturate(_Multiplier * sqrt(hr * hr + vt * vt));
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                float edge = 1 - saturate(sobel(_CameraDepthTexture, i.uv));
                return col * edge;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
