Shader "Custom/Breakables"
{
    Properties
    {
        _Color("Main Color", Color) = (1,0.5,0.5,1)
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Crack1("Crack 1", 2D) = "white" {}
        _CrackIndex("Index", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert

        float4 _Color;
        float4 _CrackColor;
        float _CrackIndex;
        sampler2D _MainTex;
        sampler2D _Crack1;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Crack1;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);

            _CrackColor = _Color * _CrackIndex;

            o.Albedo = c.rgb * _Color;
            //o.Albedo = cracks.rgb;
            o.Alpha = c.a;

            o.Emission = tex2D(_Crack1, IN.uv_Crack1) * _CrackColor.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}