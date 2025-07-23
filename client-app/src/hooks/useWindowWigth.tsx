import { useEffect, useState } from "react";

export default function useWindowWigth() {
    const [windowWigth, setWindowWidth] = useState(window.innerWidth);
    useEffect(() => {
        const handleResize = () => { setWindowWidth(window.innerWidth); }
        window.addEventListener('resize', handleResize);
        return () => { window.removeEventListener('resize', handleResize); }
    }, [])
    return windowWigth;
}