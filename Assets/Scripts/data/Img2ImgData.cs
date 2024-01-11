namespace data
{
    [System.Serializable]
    public class Img2ImgData
    {
        public string prompt;
        public string negative_prompt;
        public string init_image;
        public int width;
        public int height;
        public int samples;
    }
}

