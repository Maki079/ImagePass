from flask import Flask, request, jsonify
from diffusers import DiffusionPipeline
import torch
from PIL import Image
import io
import base64
from pymongo.mongo_client import MongoClient
from bson import ObjectId
import threading
import queue

uri = "mongodb://localhost:27017"



#  connect to the DB
client = MongoClient(uri)
dbImagePass = client['imagePass']  
collectionImagePass = dbImagePass['imageGen']  


# load the pipeline globally 
pipeline = DiffusionPipeline.from_pretrained("runwayml/stable-diffusion-v1-5", torch_dtype=torch.float16)
pipeline.to("cuda")


app = Flask(__name__)

task_queue = queue.Queue()

def process_image():
    while True:

        task = task_queue.get()
        prompt,image_id,steps = task['prompt'], task['imageId'], task['steps']


        output = pipeline(prompt, num_inference_steps=steps)
        image = output.images[0]
    
        if not isinstance(image, Image.Image):
            pil_image = Image.fromarray(image.cpu().numpy().astype("uint8"))
        else:
            pil_image = image
    
        buffer = io.BytesIO()
        pil_image.save(buffer, format="PNG")
        img_str = base64.b64encode(buffer.getvalue()).decode()
    
        # save to MongoDB
        collectionImagePass.update_one({"_id": ObjectId(image_id)}, {"$set": {"ImageEncodedInBase64": img_str}},upsert=True)
        
        task_queue.task_done()




@app.route('/generate-image', methods=['POST'])
def generate_image():

    if not request.json or 'prompt' not in request.json:
        return jsonify({'error': 'Please provide a prompt in JSON format.'}), 400

    prompt = request.json['prompt']
    image_id = request.json['imageId']
    steps = request.json.get('steps')

    try:
        # try to create ObjectId out of image_id
        ObjectId(image_id)
    except:
        return jsonify({'error': 'Invalid imageId, must be a valid ObjectId string.'}), 400



    # put the task to queue
    task_queue.put({'prompt': prompt, 'imageId': image_id, 'steps': steps})

    return jsonify({'message': 'Image generation in process', 'imageId' : image_id}), 202




worker_thread = threading.Thread(target=process_image, daemon=True)
worker_thread.start()

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)
