Shader "Custom/FogOfWar" {

	Properties 
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Blur("Blur", Range(0,0.003)) = 0.0015
	}
	
	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "LightMode"="ForwardBase" }
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert alpha
		#pragma target 3.0
		
		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, float aten)
		{
			fixed4 color;
			color.rgb = s.Albedo;
			color.a = s.Alpha;
			return color;
		}

		fixed4 _Color;
		sampler2D _MainTex;
		float _Blur;

		struct Input 
		{
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
		        half4 baseColor1 = tex2D (_MainTex, IN.uv_MainTex + float2(-_Blur, 0));
			half4 baseColor2 = tex2D (_MainTex, IN.uv_MainTex + float2(0, -_Blur));
			half4 baseColor3 = tex2D (_MainTex, IN.uv_MainTex + float2(_Blur, 0));
			half4 baseColor4 = tex2D (_MainTex, IN.uv_MainTex + float2(0, _Blur));
			half4 baseColor = 0.1 * (baseColor1 + baseColor2 + baseColor3 + baseColor4);
			o.Albedo = _Color.rgb * baseColor.b;
			o.Alpha = _Color.a - baseColor.g;
		}
		
		ENDCG
	} 
	Fallback "Diffuse"
}
