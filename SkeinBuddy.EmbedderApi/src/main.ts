import cors from 'cors'
import express, { Express } from 'express'
import { embed } from './embedder.js';

const app: Express = express();
app.use(express.json());

app.use(cors());
app.use(function (_req, res, next) {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept');
    next();
});

app.get('/', (req, res) => {
    res.json({ status: 'OK' });
});

app.post('/embed', async (req, res) => {
    let embeddings = await embed(req.body.text as string[])

    res.setHeader('Content-Type', 'application/octet-stream');
    res.setHeader('Content-Length', embeddings.length);
    
    res.send(embeddings);
});

const server = app.listen(8080, () => {
    console.log('Server running on port 8080')
});