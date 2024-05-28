//-----------------------------------------------------------------------------
// Copyright(C) Yan Verde - All Rights Reserved
// Copyright protected under Unity Asset Store EULA
// Refer to https://unity3d.com/legal/as_terms for more informations
// -----------------------------------------------------------------------------
// URP Water
// Author : Yan Verde
// Date : April 10, 2021
// -----------------------------------------------------------------------------

#ifndef URPWATER_REFRACTION_INCLUDED
#define URPWATER_REFRACTION_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#include "URPWaterVariables.hlsl"
#include "URPWaterHelpers.hlsl"

//UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraOpaqueTexture);
//TEXTURE2D(_CameraGBufferTexture2);

void ComputeOpaqueAndDepth(inout GlobalData data, out float4 clearData, out float4 refractionData, out float2 refractionOffset)
{

	// ======================================================================
	// Check if better way here:
	//https://catlikecoding.com/unity/tutorials/flow/looking-through-water/

	// Clean Data
	//clearData.rgb = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture_linear_clamp, data.screenUV.xy).rgb; // Color
	float2 screenUV = data.screenUV.xy;

	//UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraOpaqueTexture, sampler_CameraOpaqueTexture_linear_clamp, screenUV);
	clearData.rgb = SampleSceneColor(screenUV); // Color
	clearData.a = SampleDepth(screenUV); // Depth


	// Distorted Data
	float2 distortionAmount = _CameraOpaqueTexture_TexelSize.xy * _Distortion.xx;

	// Far Distortion
	float farDistance = saturate(1 - length(data.worldPosition.rgb - _WorldSpaceCameraPos.xyz) / 50);
	distortionAmount = lerp(distortionAmount * 0.25, distortionAmount, farDistance);


	#if _DOUBLE_SIDED_ON
	distortionAmount = lerp(distortionAmount * 0.5, distortionAmount, data.vFace);
	#endif

	float2 offset = data.worldNormal.xz * distortionAmount;
	float2 GrabUV = OffsetUV(data.screenUV, offset);
	float2 DepthUV = OffsetDepth(data.screenUV, offset);

	float4 distortedData;
	//distortedData.rgb = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraOpaqueTexture, GrabUV.xy).rgb
	//distortedData.rgb = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture_linear_clamp, GrabUV).rgb; // Color
	//UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraOpaqueTexture, sampler_CameraOpaqueTexture_linear_clamp, GrabUV);

	float rawDepth = SampleRawDepth(DepthUV);

	distortedData.rgb = SampleSceneColor(GrabUV); // Color
	distortedData.a = RawDepthToLinear(rawDepth);//SampleDepth(DepthUV); // Depth
	
	refractionData = data.pixelDepth > distortedData.a ? clearData : distortedData;
	refractionOffset = offset;


	data.refractionUV = DepthUV;
	data.rawDepthDst = rawDepth;
}


void ComputeRefractionData(inout GlobalData data)
{
	float4 clearData;
	float4 refractionData;
	float2 refractionOffset;
	float rawDepth;
	float3 subNormal;

	ComputeOpaqueAndDepth(data, clearData, refractionData, refractionOffset);
	data.depth = DistanceFade(refractionData.a, data.pixelDepth, _DepthStart, _DepthEnd);
	
	// Compositing in lighting
	data.refractionData = refractionData;
	data.clearColor.rgb = clearData.rgb;
	data.sceneDepth = clearData.a;
	data.refractionOffset = refractionOffset;
}

#endif