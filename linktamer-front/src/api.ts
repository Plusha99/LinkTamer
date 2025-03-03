const API_BASE_URL = process.env.REACT_APP_API_URL;

export const shortenUrl = async (originalUrl: string) => {
  const response = await fetch(`${API_BASE_URL}/shorten`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ originalUrl })
  });
  if (!response.ok) {
    throw new Error("Failed to shorten URL");
  }
  return response.json();
};

export const getUrlStats = async (shortCode: string) => {
    const response = await fetch(`${API_BASE_URL}/stats/${encodeURIComponent(shortCode)}`);
    if (!response.ok) {
      throw new Error("Failed to fetch stats");
    }
    return response.json();
  };    
