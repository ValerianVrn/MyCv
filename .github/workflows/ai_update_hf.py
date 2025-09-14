import os
from huggingface_hub import InferenceClient

hf_token = os.environ["HF_TOKEN"]

client = InferenceClient(
    api_key=hf_token,
    provider="auto"
)

candidates = [
    "meta-llama/Llama-3.1-8B-Instruct",
    "deepseek-ai/DeepSeek-R1",
    "Qwen/Qwen2.5-7B-Instruct",
    "HuggingFaceTB/SmolLM3-3B",
    # add more from your filtered list
]

prompt = "Translate this instruction into a MudBlazor page redesign suggestion: Make a clean modern layout with responsive grid."

for model in candidates:
    try:
        resp = client.chat.completions.create(
            model=model,
            messages=[{"role":"user", "content": prompt}]
        )
        text = resp.choices[0].message.content
        print(f"✅ Model {model} works:\n{text[:200]}\n")
    except Exception as e:
        print(f"❌ Model {model} failed: {e}\n")