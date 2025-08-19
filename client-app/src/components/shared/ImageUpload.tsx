import { useEffect, useState } from "react"
import { MinorButton } from "./Buttons";
import { OnSurfaceBlock } from "./Blocks";

interface ImageUploadProps {
    onChange:(image:File) => void;
    className?: string;
}
export default function ImageUpload({onChange, className}:ImageUploadProps) {
    const [file, setFile] = useState<File|null>(null);
    const [imgUrl, setImageUrl] = useState<string | null>(null);

    const handleFileSelection = (files: FileList|null) => {
        if(files && files.length > 0){
            const sizeLimit = 2 * 1024 * 1024; // 2 MB
            if(files[0].size > sizeLimit){
                alert("Image needs to be under 2MB!");
                return;
            }
            setFile(files[0]);
            onChange(files[0]);
        }
    }

    useEffect(() => {
        // Generate local URL
        if (!file) {
            setImageUrl(null);
            return;
        }
        const objectUrl = URL.createObjectURL(file);
        setImageUrl(objectUrl);

        // Free memory on re-render 
        return () => { 
            console.log("Clean-up"); 
            URL.revokeObjectURL(objectUrl) 
        };
    }, [file]);

    return <OnSurfaceBlock className={"flex flex-col items-center " + className}>
        {imgUrl && <img src={imgUrl} 
            className="mb-5 rounded-[0.5rem] bg-black border-2 cus-outline h-[250px] w-[250px]" />}
        <input
            id="file-upload"
            type="file"
            accept="image/*"
            onChange={(e) => handleFileSelection(e.target.files)}
            className="hidden"
        />
        <label htmlFor="file-upload">
            <MinorButton onClick={()=>{}}>Select Image</MinorButton>
        </label>
    </OnSurfaceBlock> 
}