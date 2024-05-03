# Dataset

## Folder Structure in Google Drive
```
Rubik's Solver
|-- config.yml
|-- dataset
|   |-- images
|   |   |-- train
|   |   |   |-- a1b2c3d4.jpg
|   |   |   |-- e5f6g7h8.jpg
|   |   |   |-- ...
|   |   |-- validation
|   |       |-- i9j0k1l2.jpg
|   |       |-- m3n4o5p6.jpg
|   |       |-- ...
|   |-- labels
|       |-- train
|       |   |-- a1b2c3d4.txt
|       |   |-- e5f6g7h8.txt
|       |   |-- ...
|       |-- validation
|           |-- i9j0k1l2.txt
|           |-- m3n4o5p6.txt
|           |-- ...
```

## Steps for Upload
Use a Google Drive CLI tool or manually perform the following steps with Google Drive web-interface.

Change the working directory to this repository.
```bash
cd vision-cube
```

Create a directory named "Rubik's Solver" if you haven't created it already.
```bash 
gdrive files mkdir "Rubik's Solver"
```

Change the parent to your own google drive "Rubik's Solver"'s directory_id. 

Upload the dataset and its corresponding config.yaml file according to above structure.
```bash
gdrive files upload ./data/dataset --recursive --parent "1mr7aP3U7xAQt2qUGxT0Dx5i_Gc9jtQMX"
```
```bash
gdrive files upload ./data/config.yaml --parent "1mr7aP3U7xAQt2qUGxT0Dx5i_Gc9jtQMX"
```
