import { useState, useEffect, useCallback } from "react";
import { fraApi } from "../api/fraApi";

export function useFRA() {
  const [fras, setFras] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchMyFRAs = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await fraApi.getMyFundraisers();
      if (res.status === 404) {
        setFras([]);
        return;
      }
      if (!res.ok) throw new Error("Failed to load fundraisers");
      const data = await res.json();
      setFras(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchMyFRAs();
  }, [fetchMyFRAs]);

  const searchFRAs = useCallback(async (name) => {
    if (!name.trim()) {
      fetchMyFRAs();
      return;
    }
    setLoading(true);
    setError(null);
    try {
      const res = await fraApi.search(name);
      if (res.status === 404) {
        setFras([]);
        return;
      }
      if (!res.ok) throw new Error("Search failed");
      const data = await res.json();
      // search returns a single object or array — normalise
      setFras(Array.isArray(data) ? data : [data]);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [fetchMyFRAs]);

  const createFRA = useCallback(async (payload) => {
    const res = await fraApi.create(payload);
    const text = await res.text();
    if (!res.ok) throw new Error(text || "Failed to create fundraiser");
    await fetchMyFRAs();
    return text;
  }, [fetchMyFRAs]);

  const updateFRA = useCallback(async (payload) => {
    const res = await fraApi.update(payload);
    const text = await res.text();
    if (!res.ok) throw new Error(text || "Failed to update fundraiser");
    await fetchMyFRAs();
    return text;
  }, [fetchMyFRAs]);

  const deleteFRA = useCallback(async (fundraiserId) => {
    const res = await fraApi.delete(fundraiserId);
    const text = await res.text();
    if (!res.ok) throw new Error(text || "Failed to delete fundraiser");
    await fetchMyFRAs();
    return text;
  }, [fetchMyFRAs]);

  return {
    fras,
    loading,
    error,
    refetch: fetchMyFRAs,
    searchFRAs,
    createFRA,
    updateFRA,
    deleteFRA,
  };
}