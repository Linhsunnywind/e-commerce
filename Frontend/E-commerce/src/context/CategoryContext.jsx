import { createContext, useContext, useState, useEffect } from 'react'
import categoryApi from '../api/categoryApi.js'

const CategoryContext = createContext(null);
export function CategoryProvider({children}){
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    useEffect(()=>{
        const fetchCategories = async () => {
            try {
                const res = await categoryApi.getAll();
                setCategories(res.data || []);
            }catch (err){
                console.error(err);
            } finally {
                setLoading(false);
            }
        }
        fetchCategories()
    }, [])

    return (
        <CategoryContext.Provider value = {{categories, loading}}>
            {children}
        </CategoryContext.Provider>
    )
}

export const useCategories = () => useContext(CategoryContext);