# Turing's Last Cipher

A text-based cipher/puzzle adventure built for the **DEV June Solstice Game Jam**.

You decrypt a series of messages that claim to be Alan Turing's last words, helped by an
in-game AI assistant whose trustworthiness erodes across four chapters. What you're really
being asked is the Turing Test in reverse: if an AI can reproduce every thought of a person,
is it that person?

**▶ Play it live:** https://turings-last-cipher-336966558985.us-central1.run.app

## Chapters

1. **Classical ciphers** — Caesar → Atbash → Vigenère; the assistant is truthful.
2. **Enigma-lite** — solved with an in-game machine you dial (historical rotors, reflector,
   plugboard, ring settings).
3. **Trust erosion** — a substitution puzzle where the assistant begins to mislead.
4. **The twist ending.**

## Architecture

A single ASP.NET Core (.NET 9) container hosts the Blazor **WebAssembly** client and exposes
`/api/*`. The game is **stateless** server-side; the server is the sole authority for solution
checking and never sends plaintext to the client.

- `src/TuringsLastCipher.Core/` — ciphers, oracle, scene/game models. Pure, deterministic, no
  I/O or LLM calls. Fully unit-tested.
- `src/TuringsLastCipher.Api/` — Minimal API: hosts the WASM client, the puzzle endpoints, and
  a server-side Gemini proxy.
- `src/TuringsLastCipher.Client/` — Blazor WASM terminal-themed UI.
- `content/scenes/story.json` — content-driven story graph.

### Design invariants

- **Ciphers are deterministic C#; the LLM never performs crypto.** Encryption, decryption, and
  answer verification live entirely in `Core`.
- **Truth lives in code, never in the model.** Every puzzle has a known plaintext; the oracle is
  a deterministic comparison. Gemini output is never trusted as ground truth.
- **The "unreliable AI" is scripted game logic.** The server decides truthful/misleading/
  withholding; Gemini only renders the wording. A deceptive hint can never make a puzzle
  unsolvable — a deterministic path to the solution always remains.
- **The API key never reaches the client.** Gemini is called only server-side; the game is
  fully playable offline via static fallbacks.

## Google AI usage

The in-game assistant's hints and dialogue are rendered by the **Gemini API** (`gemini-2.5-flash`)
through a server-side proxy, under a server-decided "trust policy". Prompts include the cipher
name, the genuine hint, and the policy — **never the plaintext** — so a rendered hint can never
leak the answer. Deployed on **Google Cloud Run**.

## Build & run

```bash
dotnet build                                          # build the solution
dotnet test                                           # run all xUnit tests
dotnet run --project src/TuringsLastCipher.Api        # run API + WASM (Development serves the client)
dotnet publish src/TuringsLastCipher.Api -c Release -o publish   # single deployable artifact
```

Set `GEMINI_API_KEY` to enable live Gemini text; without it the game runs on static fallbacks.

### Deploy (Google Cloud Run)

```bash
gcloud run deploy turings-last-cipher --source . --region us-central1 \
  --allow-unauthenticated --set-env-vars GEMINI_API_KEY=...
```

The root `Dockerfile` builds the multi-stage container; the server reads `PORT` and binds
`0.0.0.0` (Cloud Run injects `PORT`).

## License

[MIT](LICENSE) © 2026 Georg Piwonka
