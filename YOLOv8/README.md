# YOLOv8
We use Google Colab to train the Cube Detection model. You can modify the code to train the model locally.

## Upload the training code to Google Drive
Create a directory named "Notebooks".
```bash
gdrive files mkdir "Notebooks"
```
Change the parent to your own google drive "Notebooks"'s directory_id. 
```bash
gdrive files upload ./YOLOv8/colab_train.ipynb --parent "1TcXuMdLQ4SLISLlV5DGsojY-Pt2aDpJG" --mime "application/vnd.google.colaboratory"
```