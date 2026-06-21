# Submission Checklist — DEV June Solstice Game Jam

**Deadline:** June 21, 2026, 23:59 PDT.
**Live game:** https://turings-last-cipher-336966558985.us-central1.run.app
**Repo:** https://github.com/gpiwonka/turings-last-cipher

---

## Done ✅

- [x] **English game** — all player-facing text is English (prize eligibility).
- [x] **Live deployment** on Google Cloud Run (single container, scale-to-zero).
- [x] **Gemini API live** — assistant hints + dialogue rendered by `gemini-2.5-flash`;
      verified Truthful (Ch1) and Misleading (Ch3) without leaking the deception mechanic.
- [x] **Public GitHub repo** with full source.
- [x] **MIT LICENSE** committed.
- [x] **README** with play link, architecture, and Google AI usage.
- [x] **DEV post draft** (English) — `submission/DEV-POST.md`.
- [x] **Video voiceover script** — `submission/VIDEO-SCRIPT.md`.
- [x] Repo cleaned (no build artifacts / IDE files committed).
- [x] All tests green (`dotnet test`).

## To do — only you can do these ⬜

- [ ] **Record the demo video with voiceover** (template requirement). Follow
      `submission/VIDEO-SCRIPT.md`; capture the shot checklist at the bottom of that file.
      Gemini is live, so an assistant line can be shown as genuinely AI-generated.
- [ ] **Upload the video** (YouTube/Vimeo or DEV's uploader) and paste the link into
      `submission/DEV-POST.md` where it says `> *[Embed your demo video here]*`.
- [ ] **Publish the DEV post** (copy from `submission/DEV-POST.md`), embedding the repo link.
- [ ] **In the post, explicitly claim the categories** and explain how each applies
      (already drafted: Best Ode to Alan Turing = mechanics + narrative; Best Google AI
      Usage = Gemini on Cloud Run). Confirm overall eligibility too.
- [ ] **Disclosure** is in the post draft (built from scratch in the jam, AI-assisted dev via
      Claude Code, Gemini for in-game wording only, MIT). Review and confirm it's accurate.

## Optional / nice-to-have

- [ ] German completion badge / localization (non-prize bonus only — never at the cost of the
      English path).
- [ ] Add GitHub repo "About" topics (e.g. `blazor`, `dotnet`, `gemini`, `cloud-run`,
      `game-jam`, `cryptography`) for discoverability.
- [ ] Move the Gemini key from a plain env var to Secret Manager
      (`--set-secrets GEMINI_API_KEY=...`) — cleaner, not required for the jam.

## Pre-submit sanity pass

- [ ] Open the live URL in a fresh browser and play Ch1 → ending once end-to-end.
- [ ] Confirm the post, the game UI, and the video voiceover are all in **English**.
- [ ] Confirm the repo is **public** and the live URL loads for a logged-out visitor.
- [ ] Double-check the submission deadline timezone (23:59 **PDT**).
