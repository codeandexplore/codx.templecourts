import { useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import { store, useAppSelector } from "../store/store";

let connection: signalR.HubConnection | null = null;
let connectPromise: Promise<void> | null = null;
const joinedThreads = new Set<string>();

function getConnection(): signalR.HubConnection {
  if (!connection) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_URL || "http://localhost:5000"}/hubs/study-session`, {
        accessTokenFactory: () => store.getState().auth.accessToken ?? "",
      })
      .withAutomaticReconnect()
      .build();
  }
  return connection;
}

function ensureConnected(): Promise<void> {
  const conn = getConnection();
  if (conn.state === "Connected") return Promise.resolve();
  if (connectPromise) return connectPromise;
  connectPromise = conn.start().catch(() => {
    connectPromise = null;
  });
  return connectPromise;
}

export function useThreadHub(threadId: string | undefined, onMessagePosted: () => void) {
  const accessToken = useAppSelector((s) => s.auth.accessToken);

  useEffect(() => {
    if (!threadId || !accessToken) return;

    const conn = getConnection();

    const handler = (data: { threadId: string }) => {
      if (data.threadId === threadId) onMessagePosted();
    };
    conn.on("ThreadMessagePosted", handler);

    ensureConnected().then(() => {
      if (!joinedThreads.has(threadId)) {
        joinedThreads.add(threadId);
        conn.invoke("JoinThread", threadId).catch(() => {});
      }
    });

    return () => {
      conn.off("ThreadMessagePosted", handler);
    };
  }, [threadId, accessToken, onMessagePosted]);
}