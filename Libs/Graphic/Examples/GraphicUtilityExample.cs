using EasyEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace MMGame.Graphic.Example
{
    public class GraphicUtilityExample : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer source;

        [SerializeField]
        private SpriteRenderer output;

        [SerializeField]
        private TextureFormat format;

        //--------------------------------------------------
        // Mosaics
        //--------------------------------------------------

        [SerializeField]
        private int mosaicSize;

        [Inspector]
        private void TestMosaics()
        {
            Vector2 size = SpriteUtility.GetRealSize(output);
            Texture2D sTex = source.sprite.texture;
            var outTex = new Texture2D(sTex.width, sTex.height, format, sTex.mipmapCount > 0);
            GraphicUtility.Mosaics(sTex, ref outTex, mosaicSize);

            Vector2 pivot = new Vector2(0.5f, 0.5f);
            output.sprite = Sprite.Create(outTex, new Rect(0f, 0f, outTex.width, outTex.height), pivot);
            output.color = Color.white;
            SpriteUtility.SetRealSize(output, size.x, size.y);
        }
    }
}