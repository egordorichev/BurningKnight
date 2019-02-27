using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;


namespace Aseprite {
	public class AsepriteReader : ContentTypeReader<AsepriteFile> {
		public static GraphicsDevice GraphicsDevice;

		protected override AsepriteFile Read(ContentReader input, AsepriteFile existingInstance) {
			if (existingInstance != null) {
				return existingInstance;
			}

			int width = input.ReadInt32();
			int height = input.ReadInt32();

			AsepriteFile aseprite = new AsepriteFile {
				Width = width,
				Height = height
			};

			// Layers
			int layersCount = input.ReadInt32();
			AsepriteLayer layer;
			List<AsepriteLayer> layersByIndex = new List<AsepriteLayer>();

			for (int i = 0; i < layersCount; i++) {
				layer = new AsepriteLayer {
					Name = input.ReadString(),
					Opacity = input.ReadSingle(),
					BlendMode = (AsepriteLayer.BlendModes) input.ReadInt32(),
					Visible = input.ReadBoolean()
				};

				aseprite.Layers.Add(layer);
				// Use this for referencing cels down further
				layersByIndex.Add(layer);
			}

			// Frames
			AsepriteFrame frame;
			AsepriteCel cel;

			int framesCount = input.ReadInt32();
			int celsCount;
			int celOriginX;
			int celOriginY;
			int celWidth;
			int celHeight;
			int pixelIndex;
			int textureWidth = framesCount * width;
			int textureHeight = layersCount * height;

			Color[] pixelData = new Color[textureWidth * textureHeight];

			for (int f = 0; f < framesCount; f++) {
				frame = new AsepriteFrame {
					Duration = input.ReadSingle()
				};

				// Cels
				celsCount = input.ReadInt32();

				for (int i = 0; i < celsCount; i++) {
					int layerIndex = input.ReadInt32();
					
					cel = new AsepriteCel {
						Layer = layersByIndex[layerIndex]
						// ClipRect = new Rectangle(f * width, layerIndex * height, width, height)
					};

					frame.Cels.Add(cel);

					// Get info for the texture
					celOriginX = input.ReadInt32();
					celOriginY = input.ReadInt32();
					celWidth = input.ReadInt32();
					celHeight = input.ReadInt32();
					
					for (int celY = celOriginY; celY < celOriginY + celHeight; celY++) {
						for (int celX = celOriginX; celX < celOriginX + celWidth; celX++) {
							var pixel = input.ReadColor();
							//           | x                  | y
							pixelIndex = (f * width) + celX + ((layerIndex * height) + celY) * textureWidth;
							pixelData[pixelIndex] = pixel;
						}
					}
				}

				aseprite.Frames.Add(frame);
			}

			// Dump our pixels into the texture
			aseprite.Texture = new Texture2D(GraphicsDevice, textureWidth, textureHeight);
			aseprite.Texture.SetData(pixelData);

			// Animations
			
			int animationsCount = input.ReadInt32();
			AsepriteAnimation animation;
			
			for (int i = 0; i < animationsCount; i++) {
				animation = new AsepriteAnimation {
					Name = input.ReadString(),
					FirstFrame = input.ReadInt32(),
					LastFrame = input.ReadInt32(),
					Directions = (AsepriteTag.LoopDirections) input.ReadByte()
				};
				
				aseprite.Animations.Add(animation.Name, animation);
			}

			// If no animations were added then just add one 
			// that covers all frames
			if (aseprite.Animations.Count == 0) {
				animation = new AsepriteAnimation {
					Name = "Idle",
					FirstFrame = 0,
					LastFrame = aseprite.Frames.Count - 1
				};
				
				aseprite.Animations.Add(animation.Name, animation);
			}

			return aseprite;
		}
	}
}