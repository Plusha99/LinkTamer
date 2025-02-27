const API_BASE_URL = process.env.REACT_APP_API_URL + "/api/urlshortener";

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

export const getUrlStats = async (shortUrl: string) => {
    const response = await fetch(`${API_BASE_URL}/stats/${encodeURIComponent(shortUrl)}`);
    if (!response.ok) {
      throw new Error("Failed to fetch stats");
    }
    return response.json();
  };  
