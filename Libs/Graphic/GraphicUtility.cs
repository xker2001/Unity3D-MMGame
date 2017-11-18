using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame
{
    public static class GraphicUtility
    {
        //--------------------------------------------------
        // 对贴图进行马赛克处理。
        //--------------------------------------------------

        /// <summary>
        /// 对贴图进行马赛克处理。
        /// </summary>
        /// <param name="source">源贴图。</param>
        /// <param name="output">输出贴图。</param>
        /// <param name="mosaicSize">马赛克大小（像素）。</param>
        public static void Mosaics(Texture2D source, ref Texture2D output, int mosaicSize)
        {
            Color[] sourceColors = source.GetPixels();
            var outputColors = new Color[output.width * output.height];

            mosaicSize = Mathf.Max(mosaicSize, 1);

            int blockCols = Mathf.CeilToInt(1.0f * output.width / mosaicSize - 0.0001f);
            int blockRow = Mathf.CeilToInt(1.0f * output.height / mosaicSize - 0.0001f);

            for (int i = 0; i < blockRow; i++)
            {
                for (int j = 0; j < blockCols; j++)
                {
                    PixelBlock outputBlock = CreateMosaicBlock(i, j, output.width, output.height, mosaicSize);
                    FillMosaicBlock(outputBlock,
                                    output.width, output.height, ref outputColors, source.width, source.height,
                                    sourceColors);
                }
            }

            output.SetPixels(outputColors);
            output.Apply();
        }

        /// <summary>
        /// 对 Sprite 进行马赛克处理。
        /// </summary>
        /// <param name="source">源 Sprite。</param>
        /// <param name="mosaicSize">马赛克大小（像素）。</param>
        /// <returns>马赛克化的 Sprite。</returns>
        public static Sprite Mosaics(Sprite source, int mosaicSize)
        {
            // TODO: 暂时不实现这个功能。该功能是为了方便支持将 UI.Image 的图片进行马赛克处理
            return new Sprite();
        }

        /// <summary>
        /// 定义一个马赛克块。
        /// </summary>
        private struct PixelBlock
        {
            public readonly int StartRow;
            public readonly int EndRow;
            public readonly int StartCol;
            public readonly int EndCol;

            public PixelBlock(int startRow, int endRow, int startCol, int endCol)
            {
                StartRow = startRow;
                EndRow = endRow;
                StartCol = startCol;
                EndCol = endCol;
            }
        }

        /// <summary>
        /// 创建一个代表马赛克块的 PixelBlock。
        /// </summary>
        /// <param name="row">马赛克块的行号。</param>
        /// <param name="col">马赛克块的列号。</param>
        /// <param name="imageWidth">图像的宽度。</param>
        /// <param name="imageHeight">图像的高度。</param>
        /// <param name="mosaicSize">马赛克块的大小（像素）。</param>
        /// <returns>马赛克块。</returns>
        private static PixelBlock CreateMosaicBlock(int row, int col, int imageWidth, int imageHeight, int mosaicSize)
        {
            int startRow = row * mosaicSize;
            int endRow = Mathf.Min(imageHeight - 1, startRow + mosaicSize - 1);
            Assert.IsTrue(endRow >= startRow, "PixelBlock: endRow < startRow!");
            int startCol = col * mosaicSize;
            int endCol = Mathf.Min(imageWidth - 1, startCol + mosaicSize - 1);
            Assert.IsTrue(endCol >= startCol, "PixelBlock: endCol < startCol!");

            return new PixelBlock(startRow, endRow, startCol, endCol);
        }

        /// <summary>
        /// 为目标贴图上的一个马赛克块填充颜色。
        /// </summary>
        /// <param name="outputBlock"></param>
        /// <param name="outputWidth">输出贴图宽度。</param>
        /// <param name="outputHeight">输出贴图高度。</param>
        /// <param name="outpuColors">输出贴图像素数组。</param>
        /// <param name="sourceWidth">源贴图宽度。</param>
        /// <param name="sourceHeight">源贴图高度。</param>
        /// <param name="sourceColors">源贴图像素数组。</param>
        private static void FillMosaicBlock(PixelBlock outputBlock,
                                            int outputWidth, int outputHeight, ref Color[] outpuColors,
                                            int sourceWidth, int sourceHeight, Color[] sourceColors)
        {
            Color blockColor = GetMosaicBlockColor(outputBlock, outputWidth, outputHeight,
                                                   sourceWidth, sourceHeight, sourceColors);

            for (int i = outputBlock.StartRow; i <= outputBlock.EndRow; i++)
            {
                for (int j = outputBlock.StartCol; j <= outputBlock.EndCol; j++)
                {
                    outpuColors[i * outputWidth + j] = blockColor;
                }
            }
        }

        /// <summary>
        /// 获取马赛克块的颜色。
        /// 该颜色为对应源贴图上像素颜色的平均值。 
        /// </summary>
        /// <param name="outputBlock">输出贴图上的马赛克块。</param>
        /// <param name="outputWidth">输出贴图的宽度。</param>
        /// <param name="outputHeight">输出贴图的高度。</param>
        /// <param name="sourceWidth">源贴图的宽度。</param>
        /// <param name="sourceHeight">源贴图的高度。</param>
        /// <param name="sourceColors">源贴图颜色数组。</param>
        /// <returns>颜色值。</returns>
        private static Color GetMosaicBlockColor(PixelBlock outputBlock, int outputWidth, int outputHeight,
                                                 int sourceWidth, int sourceHeight, Color[] sourceColors)
        {
            float factH = 1.0f * sourceHeight / outputHeight;
            float factW = 1.0f * sourceWidth / outputWidth;
            int sourceStartRow = Mathf.FloorToInt(outputBlock.StartRow * factH + 0.0001f);
            int sourceEndRow = Mathf.CeilToInt(outputBlock.EndRow * factH - 0.0001f);
            int sourceStartCol = Mathf.FloorToInt(outputBlock.StartCol * factW + 0.0001f);
            int sourceEndCol = Mathf.CeilToInt(outputBlock.EndCol * factW - 0.0001f);

            float r = 0;
            float g = 0;
            float b = 0;
            float a = 0;

            for (int i = sourceStartRow; i <= sourceEndRow; i++)
            {
                for (int j = sourceStartCol; j <= sourceEndCol; j++)
                {
                    int sourceIndex = i * sourceWidth + j;
                    r += sourceColors[sourceIndex].r;
                    g += sourceColors[sourceIndex].g;
                    b += sourceColors[sourceIndex].b;
                    a += sourceColors[sourceIndex].a;
                }
            }

            int number = (sourceEndRow - sourceStartRow + 1) * (sourceEndCol - sourceStartCol + 1);
            return new Color(r / number, g / number, b / number, a / number);
        }
    }
}