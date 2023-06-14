import os
import zipfile

blacklistedFiles = ["Microsoft.Win32.TaskScheduler.resources.dll"]
blacklistedExtensions = ["config","pdb","xml"]

def add_files_to_zip(zip_file, source_dir, added_files):
    for root, _, files in os.walk(source_dir):
        for file in files:
            if file in blacklistedFiles: continue # stupit tasks library loves creating these
            if file.split(".")[-1] in blacklistedExtensions: continue
            file_path = os.path.join(root, file)
            if file not in added_files:
                zip_file.write(file_path, os.path.relpath(file_path, source_dir))
                added_files.add(file)

source_dir1 = "MLock/bin/x86/Release"
source_dir2 = "MLockConfigurator/bin/x86/Release"
destination_dir = "."
zip_file_name = "Release.zip"

zip_file_path = os.path.join(destination_dir, zip_file_name)
added_files = set()

with zipfile.ZipFile(zip_file_path, 'w') as zip_file:
    add_files_to_zip(zip_file, source_dir1, added_files)
    add_files_to_zip(zip_file, source_dir2, added_files)

print("Files merged successfully. Zip file created at:", zip_file_path)
