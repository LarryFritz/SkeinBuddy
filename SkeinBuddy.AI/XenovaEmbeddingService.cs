using System.Collections;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace SkeinBuddy.AI
{
    public class XenovaEmbeddingService
    {
        const int TENSOR_SIZE = 384;

        private HttpClient _httpClient = new HttpClient();

        public XenovaEmbeddingService()
        {

        }

        public async Task<List<float[]>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync<object>($"http://localhost:8080/embed", new { text=data }, cancellationToken);

            byte[] byteArray = await response.Content.ReadAsByteArrayAsync();

            int dimensions = byteArray.Length / (TENSOR_SIZE * sizeof(float));

            List<float[]> embeddings = new List<float[]>();

            for(int i = 0; i < dimensions; i++)
            {
                float[] floatArray = new float[byteArray.Length / dimensions / sizeof(float)];
                Buffer.BlockCopy(byteArray, i * TENSOR_SIZE * sizeof(float), floatArray, 0, byteArray.Length / dimensions);
                embeddings.Add(floatArray);
            }

            return embeddings.ToList();
        }
    }
}
