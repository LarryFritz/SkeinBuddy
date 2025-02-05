import { pipeline } from '@huggingface/transformers'

const extractor = await pipeline('feature-extraction', 'Xenova/all-MiniLM-L6-v2', { dtype: 'fp32' })

async function embed(texts: string[]): Promise<Buffer> {
  const tensor = await extractor(texts, { pooling: 'mean', normalize: true });
  const rawData = tensor.data as Float32Array
  return Buffer.from(rawData.buffer);
}

/* Export the embed function */
export { embed }