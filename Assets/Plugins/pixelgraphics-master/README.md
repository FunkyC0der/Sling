# PixelGraphics

[![Made with Unity](https://img.shields.io/badge/Made%20with-Unity-57b9d3.svg?style=flat&logo=unity)](https://unity3d.com)
[![semantic-release](https://img.shields.io/badge/%20%20%F0%9F%93%A6%F0%9F%9A%80-semantic--release-e10079.svg)](https://github.com/semantic-release/semantic-release)

[![Patreon](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Fshieldsio-patreon.vercel.app%2Fapi%3Fusername%3Daarthificial%26type%3Dpatrons&style=for-the-badge)](https://www.patreon.com/aarthificial)

PixelGraphics is a collection of interactive foliage shaders for Unity<br>
\_Tested on Unity 6 - PC (Just Velocity Camera component)

### Installation

Pixel Graphics can be [installed as a Unity package](https://docs.unity3d.com/Manual/upm-ui-giturl.html)
using this url:

```
https://github.com/HackingCZE/pixelgraphics.git
```

### Optional: ShaderGraph Unbatcher

If you need to disable SRP Batching for specific foliage interactions or optimization reasons (e.g. to ensure individual material property updates), you can use the [**ShaderGraph Unbatcher**](https://github.com/HackingCZE/unity-shadergraph-unbatcher) tool. It allows you to export any Shader Graph as a standalone shader with batching disabled.

**Optional Tool URL:**

```
https://github.com/HackingCZE/unity-shadergraph-unbatcher.git
```

### Usage

1. [Set up a velocity texture](./Documentation~/velocity.md)
1. [Place some emitters in your scene](./Documentation~/emitters.md)
1. [Create materials using the available shaders](./Documentation~/shaders.md)
1. [Or create your own shaders](./Documentation~/properties.md)

### Release videos:
[![Making Foliage React to Player and Enemies | Foliage shader in Unity | Devlog #1](https://img.youtube.com/vi/mQmJsU14IvY/maxresdefault.jpg)](https://youtu.be/mQmJsU14IvY)
[![Interactive Foliage Shaders for Unity](https://img.youtube.com/vi/ecYWvfMoRIM/maxresdefault.jpg)](https://youtu.be/ecYWvfMoRIM)
