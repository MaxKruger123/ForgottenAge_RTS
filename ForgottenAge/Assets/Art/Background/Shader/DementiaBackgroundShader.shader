Shader "Unlit/DementiaBackgroundShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
    }
        SubShader
        {
            Tags {"Queue" = "Background" "RenderType" = "Opaque"}
            LOD 100

            CGPROGRAM
            #pragma surface surf Unlit

            sampler2D _MainTex;
            fixed4 _Color;

            struct Input
            {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutput o)
            {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }

            inline fixed4 LightingUnlit(SurfaceOutput s, fixed3 lightDir, fixed atten)
            {
                return fixed4(s.Albedo, s.Alpha);
            }
            ENDCG
        }
            FallBack "Unlit/Texture"
}
