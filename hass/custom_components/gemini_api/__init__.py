#imports
import json
import socket
import ast
import base64

DOMAIN = 'gemini_api'

CONF_API_KEY = 'api_key'

GEMINI_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key="

def setup(hass, config):
    """Service to send prompt and images to gemini."""
    api_key = config[DOMAIN][CONF_API_KEY]
    gemini_full_url = GEMINI_URL + apiKey

    def generate_text(call):
        prompt = call.data.get('prompt')
        images = call.data.get('images')

        image_base64 = [base64.b64encode(open(image, "rb").read()).decode('utf-8') for image in images]

        parts = [{"text": prompt}]
        if images:
            for image_base64 in image_base64:
                parts.append({
                    "inline_data": {
                    "mime_type": "image/jpeg",
                    "data": image_base64
                    }
                })

        content = {
            "contents": [{
            "parts": parts
            }]
        }

        headers = {
            'Content-Type': 'application/json'
        }

        response = requests.post(gemini_full_url, headers=headers, data=json.dumps(content))

        if response.status_code != 200:
          raise Exception(f"Error {response.status_code}: {response.text}")

        return {
            "text": response.json()["candidates"][0]["content"]["parts"][0]["text"]
        }


    hass.services.register(DOMAIN, 'generate_text', generate_text, supports_response=SupportsResponse.ONLY)
    return True
