Toony Colors Pro, version 2.3
2018/11/15
© 2018 - Jean Moreno
=============================

QUICK START
-----------
Select one of the following shader in your Material:
- Toony Colors Pro 2/Desktop
- Toony Colors Pro 2/Mobile
- Toony Colors Pro 2/Standard PBS
- Toony Colors Pro 2/Standard PBS (Specular)
Then select the features you want to enable (bump, specular, rim...), and the correct shader will automatically
be selected for you.
Use the (?) buttons to see help for specific features, or read the documentation for more information.

PLEASE LEAVE A REVIEW OR RATE THE PACKAGE IF YOU FIND IT USEFUL!
Enjoy! :)


CONTACT
-------
Questions, suggestions, help needed?
Contact me at:
jean.moreno.public+unity@gmail.com

I'd be happy to see Toony Colors Pro 2 used in your project, so feel free to drop me a line about that! :)


UPDATE NOTES
------------
2.3.57
- Shader Generator: upgraded "SRP/Lightweight" template to latest version (4.1.0-preview with Unity 2018.3.0b9)
- Shader Generator: "Default" template: fixed "Pixel MatCap" option when using a normal map with skinned meshes
- Shader Generator: always start with a new shader when opening the tool (don't load the last generated/loaded shader anymore)
- Added example MatCap textures
- Removed 'JMOAssets.dll', became obsolete with the Asset Store update notification system

2.3.56
- Shader Generator: added "Texture Blending" feature for "Surface PBS" template
- Shader Generator: fixed non-repeating tiling for "Texture Blending" in relevant templates
- Shader Generator: fixed masks for "Surface PBS" template (Albedo Color Mask, HSV Mask, Subsurface Scattering Mask)
- Added default non-repeating tiling noise textures

2.3.55
- Shader Generator: added "Silhouette Pass" option: simple solid color silhouette for when objects are behind obstacles
- Shader Generator: fixed fog for "Standard PBS" shaders in Unity 2018.2+
- Reorganized some files and folders

2.3.54
- Shader Generator: added more Tessellation options for "Default" template: Fixed, Distance Based and Edge Length Based
- Shader Generator: added Tessellation support for "Standard PBS" template
- Desktop/Mobile shaders: removed Directional Lightmap support for shaders, so that all variants can compile properly (max number of interpolators was reached for some combination in Unity 2018+)
- Mpbile shaders: disabled view direction calculated in the vertex shader, will be calculated in the fragment instead, so that all variants compile properly (slightly slower but it frees up one interpolator)
- Shader Generator: restored 'VertexLit' fallback for Surface PBS template, so that shadow receiving works by default

2.3.53
- Shader Generator: added "Shadow Color Mode" feature with "Replace Color" option to entirely replace the Albedo with the shadow color
- Shader Generator: updated GGX Specular to be more faithful to the Standard shader implementation
- Shader Generator: fixed GGX Specular when using Linear color space
- Shader Generator: updated Lightweight SRP template to work with Unity 2018.2 and lightweight 2.0.5-preview
- Shader Generator: Lightweight SRP template still works with Unity 2017.1 and lightweight 1.1.11-preview

2.3.52
- Shader Generator: added "Vertical Fog" option for "Default" template
- Shader Generator: fixed wrong colors and transparency in "Fade" mode with "Surface Standard PBS" template
- Shader Generator: fixed disabling depth buffer writing mode depending on transparency mode with "Surface Standard PBS" template
- Shader Generator: reorganized templates UI in foldout boxes for clarity
- Shader Generator: updated UI for clarity
- Shader Generator: harmonized UI colors for Unity Editor pro skin

2.3.51
- Shader Generator: fixed issue with "Sketch" option in "Surface Standard PBS" template
- Shader Generator: fixed "Bump Scale" option in "SRP/Lightweight" template

2.3.5
- Shader Generator: added experiment "SRP/Lightweight" template with limited features (Unity 2018.1+)
	- extract "Lightweight SRP Template.unitypackage" for it to work
- Shader Generator: added "LOD Group Blending" feature (dither or cross-fade)
- script warning fix

2.3.41
- Shader Generator: "Standard Surface PBS" template: fixed "Occlusion" and added occlusion map color channel selection
- Shader Generator: "Standard Surface PBS" template: fixed "Backface Lighting" and "Shader Target" features
- Shader Generator: "Standard Surface PBS" template: fixed transparent modes (Cutout, Fade, Transparent)
- Shader Generator: "Standard Surface PBS" template: added surface shader flags

2.3.4
- Shader Generator: new template "Standard Surface PBS" to replace the previous "Standard PBS"
	- now uses Unity's surface shader system
	- simpler and more flexible shader code
	- all base Standard shader options are now optional (better stripping and faster compilation times)
	- now embeds the outline code as in the "Default" template (use "Legacy Outline" option for old behavior)
	- all outline options from the "Default" template are now available for the "Standard Surface PBS" template
	- previous template still available as "Legacy/Standard PBS"
- Updated "Demo TCP2 Cat" scene so that it looks good in both Gamma and  Linear color space mode
- Updated "Demo TCP2 ShaderGenerator" PBS shaders to use the new template
- 2018.1 scripts compatibility

2.3.391
- Shader Generator: fixed "Backface Lighting" for Standard PBS template (it was only flipping the normal Z axis, now it flips all XYZ axis)
- Shader Generator: fixed error when using Triplanar mapping and other features using the main texture UVs
- Shader Generator: fixed compilation error when using lightmaps with some shaders
- dropped support for Unity 5.4 and lower (older versions still available to download)

2.3.39
- Shader Generator: added "Snow Accumulation" layer feature with color, vertex displacement and other options
- Shader Generator: added options for the "Occlusion Map" feature:
	- Color channel selection
	- RGB Occlusion
	- Intensity slider
- Shader Generator: added ZWrite and Cull Mode options
- Shader Generator: added "Disable Dynamic Batching" flag ("Default" and "Water" templates)
- Shader Generator: added "Backfaces Color" option for "Water" template
- Shader Generator: added "Hide Foam on Backfaces" option for "Water" template
- Shader Generator: fixed "Occlusion Map" in "Default" template
- Shader Generator: fixed "Curved World" template
- Shader Generator: fixed some shaders not compiling for OpenGL ES 2.0 with PBS template
- Shader Generator: fixed 'Transparent' Rendering Mode broken with PBS template

2.3.382
- Shader Generator: added "Emission Texture (RGB)" feature
- Shader Generator: added 3 optional masked colors
- Shader Generator: added "Phong Tessellation" feature (warning: does not work with vertex-based features)
- Shader Generator: added "Dithered Transparency" option under "Alpha Testing (Cutout)" feature (enables transparency similar to Super Mario Odyssey)
- Mobile: fixed Specular shaders not compiling when building and thus failing the build process
- Desktop/Mobile shaders: disable baked lightmaps support for shaders that run out of vertex interpolators (instead of disabling fog)
- Removed some unused shader keywords

2.3.381
- Shader Generator: fixed "Standard PBS" template

2.3.38
- added GPU Instancing support for Outline passes (all shaders)
- fixed GPU Instancing for PBS shaders (both regular and generated)
- Shader Generator: added "Shadow/Depth Pass" option for outline, so that its thickness is included in cast shadows, and also in the depth texture (e.g. for post effects like depth of field)
- Shader Generator: Specular shader and terminology fixes ("roughness"/"smoothness")

2.3.371
- Shader Generator: fixed roughness mask for all specular models
- Shader Generator: improved GGX specular (more alike PBS shaders)

2.3.37
- New demo scene "Demo TCP2 Cat" showcasing features from the Shader Generator
- Shader Generator: added different specular models option:
	- Legacy (Blinn-Phong)
	- PBR Blinn-Phong (new one where the smoothness affects the intensity)
	- GGX (different look for the highlights, also PBR)
- Shader Generator: added different cartoon specular options:
	- Smoothstep (existing one)
	- Banded (new one that divides the specular highlights into bands)
- Shader Generator: added "Triplanar Normal Maps" option
- Shader Generator: added "Shadow HSV" option (control HSV values on shadowed areas)
- Shader Generator: added "Final HSV" option (control HSV values on final pixel color, after lighting has been applied)
- Shader Generator: added "Saturation only" option to HSV control (faster, and preserves luminosity better)
- Shader Generator: added "Diffuse Tint Mask" option
- Shader Generator: fixed Point & Spot falloff texture bypassing for Unity 5.6+
- Shader Generator: fixed "Rim: Light-based Mask" not working with fog (moved out of Final Color function to Lighting function)
- Shader Generator: fixed saturation not being really grayscale when at -1 (with HSV controls)

2.3.36
- Outline Shader update:
	* fixed Z Correction not working properly compared to previous Unity versions
	* Shader Generator: embed outline shader code directly in the generated shader:
		-> no more dependencies to "TCP2_Outline_Include.cginc"
		-> will make it easier to add outline features
		-> added "Legacy Outline" option to revert to old behavior
	* Shader Generator: outline width is now more consistent with Z Correction
- Shader Generator: added "HDR Outline Color" option
- Shader Generator: added "Outline as fake rim" option (simulate a crisp rim light)
- Shader Generator: added "Vertex Color Width" option (modulate outline width with a vertex color)
- Shader Generator: added "Gamma to Linear Space" option for Vertex Colors feature
- Shader Generator ("Water" Template): reduce depth-buffer bleeding for objects in front of water (especially when a normal map is used)
- minor bug fixes in Shader Generator

2.3.35
- Smoothed Normals Utility:
	* added option to save meshes to custom directory
	* fixed error when original mesh name has invalid filename characters
- added "Sharpen Alpha-to-Coverage" option to enhance alpha testing when using MSAA and alpha-to-coverage
- fixed "Vertex Colors" option not working when selected as a mask

2.3.341
- added Textured Threshold option for Terrain template
- Terrain template lighting isn't wrapped by default anymore ("Enable Wrapped Lighting" option)
- fixed alpha transparency used with specular in "Default" template
- fixed vertex function not working for generated shaders in Unity 5.4

2.3.34
- updated "Curved World Water" template to match "Water" template
- added 'Shadowmask Support' option for "Default" templates
- fixed GPU instancing for generated shaders using the vertex function
- fixed Curved World template
- fixed sketch effect for Standard PBS template: it will now properly take cast shadows into account

2.3.33
- added 'Blending' option for the 'Color Mask' feature: set the blending of the masked color (replace, multiply or add)
	- also added to 'Standard PBS' template ('Color Mask' now always uses a separate color for this template)
- added 'Shadow Color Texture' feature: use a texture to define the shadow color
- added 'Specular Color Texture' feature: use a texture to define the specular color
- added 'Rim Color Texture' feature: use a texture to define the rim effects color
- added 'Non-Repeating Tiling' options for textures (main and terrain textures, prevents repeating pattern for tiling textures)
- added 'Stencil Buffer' option for the 'Outline Behind' feature: different technique that can prevent certain artifacts
- added 'Enable Instancing' option in material inspectors (Unity 5.6+)
- updated the 'Backface Lighting' option for double-sided materials: choose between flipping the Z or XYZ components of the normal for backfaces
- fix: generated transparent shaders should now work properly with secondary lights
- fix: outline normals selection UI now shows for shaders made with 'Standard PBS' template
- TCP2_GetPosOnWater update:
	- fixed position calculation when using 1 sine function
	- added ability to calculate the wave normal based on a position
- removed API upgrade prompt when importing in Unity 2017.1+ projects

2.3.32
- Shader Generator: added Triplanar feature for Terrain template
- Shader Generator: HSV control should now work properly with HDR values
- fixed Terrain template when height-based blending and normal maps were both enabled

2.3.31
- disabled debug mode in Shader Generator (was mistakenly left enabled in the previous update)

2.3.3
- Shader Generator: updated template to use Unity 5's surface shader model by default
 * this will improve compatibility with built-in features (e.g. Light Probe Proxy Volumes)
 * visuals should stay the same as they were, but you can use Unity 4 model if that's not the case
 * please send me a bug report if you notice something wrong
- Shader Generator: major update to the Texture Blending feature:
 * now based on vertex colors or texture map
 * can blend up to 5 textures using RGBA + black color
 * height-based blending for more realistic transition between textures (also added to Terrain template)
- Shader Generator: added Triplanar mapping option
- Shader Generator: added "Ramp Control" and "Ramp Style" features:
 * separated ramp controls per light type, or main/secondary lights
 * "RGB Slider" option, allows a different threshold per color channel (good for skin subsurface approximation)
 * shaders using the "Separated Ramps" option will switch to these new options
- Shader Generator: added Point and Spot light built-in falloff textures bypass:
 * will remove the usage of Unity's built-in falloff textures
 * ramp settings will be based on a linear falloff, based on the light's settings
 * you can define a custom falloff texture when combining this with the new Ramp Control feature
- Shader Generator: added Alpha To Coverage transparency option
- Shader Generator: added custom Light Wrapping factor option
- Shader Generator: added Specular workflow option (Standard PBS Template)
- Shader Generator: added ignore main texture and/or color alpha options for Alpha Blending & Testing
- Shader Generator: Subsurface Scattering is always using the Light's color now
- Shader Generator: added Gradient Ramp option for Dissolve
- Shader Generator: Dissolve doesn't clip by itself now, you need to enable Alpha Testing (Cutout) to get the old behavior
- Shader Generator: added UV1 option for Masks' independent UVs (to use secondary UVs from the mesh)
- Shader Generator: added height-based blending for Terrain template
- Shader Generator UI: reorganized sections with labels
- Scripts: TCP2_GetPosOnWater doesn't update the custom time value anymore, use TCP2_ShaderUpdateUnityTime instead
- Shaders: new default color values for highlight/shadows, should look more like the Default material
- Material Inspectors UI improvements
- updated documentation and reorganized sections to match Shader Generator
- fixed harmless console errors related to material inspector (hopefully!)

2.3.22
- removed harmless shader warnings in Unity 5.6

2.3.21
- fixed packed shaders file that was corrupted (unpacked shaders weren't compiling)

2.3.2
- added "Curved World" compatible Water shader template
- added direct editing of ramp textures gradient (when generated with the Ramp Generator)
- updated material inspector UI for ramp textures

2.3.11
- added script "TCP2_CameraDepth" to force rendering of the depth texture for a Camera (needed for some water shaders)
- added a warning in the material about the need for the depth texture for relevant water shaders
- fixed generated shaders include path if "Toony Colors Pro" folder has been moved

2.3.1
- PBS Shaders: Unity 5.6 compatibility
- PBS Shader Template: Unity 5.6 compatibility

2.3
- added "Water Template" for Shader Generator: make your own stylized water shader
- added "PBS Template" for Shader Generator: make your own enhanced PBS stylized shader
- added Scene "Demo TCP2 ShaderGenerator" with examples of user-generated shaders
- Shader Generator: new button to easily load included shader templates
- Shader Generator: added "Occlusion Map" feature
- Shader Generator: added "No Shadow Color" feature for secondary lights
- Shader Generator: replaced "Disable Wrapped Lighting" with "Enable Wrapped Lighting" (so that it is disabled by default)
- fix: "Standard PBS" shaders should reflect Unity 5.5's Standard shader better
- fix: Shader Generator should be much faster at loading shader list
- fix: "Backface Lighting" should work correctly now
- fix: Screen space UV offset for Sketch effects is more accurate and now correct in Scene view
- renamed texture files that were using an old prefix (from TG_ to TCP2_)
- dropped support for Unity 5.3 and lower (older versions still available to download)

2.2.6
- added "Dissolve Map" feature: simple dissolution effect based on a grayscale map and a dissolve value
- added "Depth Pass" option for "Outline behind model": this should help with sorting issues when using that option
- added "Backface Lighting" option when disabling backface culling (experimental)
- added "View Dependent" option for "Directional Ambient" feature
- added "Subsurface Light Color" option for "Subsurface Scattering" feature
- added "Separate Color" option for the "Color Mask" feature
- updated Shader Generator and Material Inspector:
 * templates now entirely handle the generated shaders' inspector UI
 * slight interface update for better clarity
- fixed "Curved World" template (was broken in previous version)

2.2.51
- Unity 5.5 compatibility
- added "HSV Controls" feature (hue/saturation/value)
- updated masks with new template system

2.2.5
- added "Diffuse Tint" option in Shader Generator
- added "Separated Ramp" option in Shader Generator
- added "Vertex Colors" option for all Masks
- major update to the Shader Generator:
 * now the whole UI is embedded into the templates (will make it easier to create different templates)
 * uses a more robust condition parser
 * the old template is still available, with the old behavior, in case the new system doesn't properly work
- fixed compilation issue on PBS shaders when UNITY_STANDARD_SIMPLE was used (it is now ignored)
- bug fixes in Terrain template

2.2.45
- added "Normal Map Blending" option for "Vertex Texture Blending" feature
- updated the "Sketch" effects in the Shader Generator:
 * the screen-space texture will be offset based on the object's position, removing the 'shower door' artefact
 * go back to the old behavior by enabling the 'Disable Obj-Space Offset' option
 * removed 'Scale with Model' option, now integrated with the new object-space offset
 * the UVs aren't multiplied with the resolution anymore, giving consistent texture scale across resolutions
- updated "Subsurface Scattering" in the Shader Generator:
 * separated Ambient Color and Color
 * additive mode is now the default behavior for more consistency, use "Multiplicative" option for previous behavior
 * fixed shadows affecting subsurface with directional lights

2.2.44
- fixed corrupted packed shaders (Unity 5.4)

2.2.43
- fixed "Use Reflection Probes" option in Desktop shader (Unity 5.4)

2.2.42
- allow Emission Color to be enabled without Emission Map in Shader Generator
- added "HDR Color" option for Emission Color in Shader Generator (useful for effects like bloom)
- renamed internal variable _IllumColor to _EmissionColor in templates
- dropped support for Unity 5.0.0 and lower (older versions still available to download)

2.2.41
- updated PBS shaders to match Standard shaders in Unity 5.4
- added "Dithered Shadows" option in Shader Generator for alpha-blended shaders
- fixed initial blending values for Alpha Blending option in Shader Generator
- minor fix to Shader Generator templates

2.2.4
- added "Subsurface Scattering" option in Shader Generator
- fixed inspectors issue with Mac retina displays

2.2.31
- added "Bump Scale" option in Shader Generator
- fixed issues when updating shaders with custom output path enabled
- removed Unity5.4 templates and added differences in the main Unity5 template

2.2.3
- added Ramp Generator utility
- added option to explicitly set shader model target in Shader Generator
- added Shader Generator template for Terrain shaders (experimental)
- removed explicit "#pragma target 2.0" in PBS shaders for lower LOD, allowing it to default to shader model target 2.5 in Unity 5.4
- renamed "Self-Illumination" feature to "Emission" in Shader Generator
- custom output path fix for already existing shaders

2.2.2
- added option to save generated shaders in custom directory

2.2.1
- added Unity 5.4 compatible templates for the Shader Generator (fix for reflection probes sampling)
- fixed seamless tiling on some sketch textures

2.2
- added "Standard PBS" version of the shaders, based on Unity Standard shaders (Unity 5.3+ only)
- added "TCP2 Demo PBS" scene to show the PBS shaders
- moved "Outline Only" shaders to their own category
- updated documentation

2.14.1
- fixed attenuation factor with "Light-based Mask" for Rim Lighting (point/spot lights, shadows)

2.14
- added "Light-based Mask" option for Rim Lighting in Shader Generator

2.13
- added option to disable wrapped diffuse lighting
- added "Colors Multipliers" option to Shader Generator
- bug fixes and improvements

2.12.2
- disabled custom lightmapping support in Unity5+, turns out it doesn't work anymore with surface shaders

2.12.1
- removed double lighting multiplication in Unity 5 template for Shader Generator
- Smoothed Normal Utility now copies blend shape data in Unity 5.3+

2.12
- added support for "Curved World" by Davit Naskidashvili in the Shader Generator (requires the "Curved World" package from the Asset Store)
- refactored outline shaders code with TCP2_Outline_Include.cginc
- fixed Blended Outline Only shaders queue (from opaque to transparent)
- fixed Outline Only material inspector glitch with slider properties in Unity 5+

2.11.2
- Mobile shader now compiles to shader model 2 as it was supposed to
- added menu options to unpack all mobile and desktop variant shaders

2.11.1
- improved detection of manually modified generated shaders
- fixed issue with Shader Generator and Texture Ramp
- fixed issue with Shader Generator and Cartoon Specular

2.11
- fixed issue with Shader Generator and TCP2 Lightmap
- added script to convert materials from TCP1 to TCP2 (in the tools menu)

2.10
- fixed issue with Smoothed Normals Utility and built-in meshes
- Smoothed Normals Utility no longer requires mesh to be read/write enabled

2.091
- fixed generated Shader userData not always saved when using Shader Generator

2.09
- added option to render outline behind the model (Shader Generator)
- fixed shader model 2 outlines with Shader Generator (Unity 4.5)

2.08
- fixed Parallax offset for diffuse texture (Shader Generator)
- fixed warnings on package import (Unity 5)
- TCP2 shaders now work correctly with Substance materials (Unity 5)

2.07
- fixed MatCap calculations (was incorrect with rotated meshes) in Mobile shaders and Shader Generator template

2.06
- fixed MatCap issue with scaled skinned meshes (added option to turn fix on/off in inspector)
- fixed Pixel MatCap breaking generated shaders if normal map was disabled

2.05
- added Pixel Matcap option in Shader Generator, allows MatCap to work with normal maps

2.04
- fixed path issues on Mac

2.03
- fixed glitched outlines in DX11

2.02
- fixed issue with vertex function in generated shaders
- removed debug information showing in material inspector

2.01
- updated Mobile shaders to target shader model 3: should fix compilation issues with some combinations, will break compatibility with super old desktop GPU (roughly pre-2004)

2.0
- everything redone from scratch!
- lots of new features and optimizations added to the shaders
- Unified Inspector: select one shader and then let the inspector choose the correct optimized shader for you
- Shader Generator: generate your own stylized shader choosing from a lot of features
- Smooth Normals Utility: generate encoded smoothed normals to fix hard-edge outlines
- new Documentation in HTML format with examples and tips

1.71
- updated "JMO Assets" menu

1.7
- added alpha output to shader files (RenderTextures should now work for real)
- Constant Outline shaders now take the object's uniform scale into account

1.6
- fixed alpha output to 0 in lighting model, would cause problems with Render Textures previously
- fixed Warnings in Unity 4+ versions
- fixed shader Warnings for D3D11 and D3D11_9X compilers
- re-enabled ZWrite by default for outlines, would cause them to not show over skyboxes previously

1.5
- fixed the specular lighting algorithm, would cause glitches with small light ranges

1.4
- changed name to "Toony Colors"
- fixed Bump Maps Substance compatibility (WARNING: you may have to re-set the Normal Maps in your materials)

1.3
- added Rim Outline shaders

1.2
- added JMO Assets menu (Window -> JMO Assets), to check for updates or get support

1.1
- Rim lighting is much faster! (excepted on Rim+Bumped shaders)

1.01
- Included Demo Scene

1.0
- Initial Release