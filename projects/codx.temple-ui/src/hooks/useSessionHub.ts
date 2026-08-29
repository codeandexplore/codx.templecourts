import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { useAppSelector } from "../store/store";

export interface SessionHubEvents {
  onQuestionReviewed?: (questionKey: string, isReviewed: boolean) => void;
  onSessionAdvanced?: (currentQuestionId: string) => void;
  onSessionEnded?: () => void;
}

export function useSessionHub(sessionId: string | undefined, events: SessionHubEvents) {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const accessToken = useAppSelector((s) => s.auth.accessToken);

  useEffect(() => {
    if (!sessionId || !accessToken) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_URL || "http://localhost:5000"}/hubs/study-session`, {
        accessTokenFactory: () => accessToken,
      })
      .withAutomaticReconnect()
      .build();

    connectionRef.current = connection;

    connection.on("QuestionReviewed", (data: { questionKey: string; isReviewed: boolean }) => {
      events.onQuestionReviewed?.(data.questionKey, data.isReviewed);
    });

    connection.on("SessionAdvanced", (data: { currentQuestionId: string }) => {
      events.onSessionAdvanced?.(data.currentQuestionId);
    });

    connection.on("SessionEnded", () => {
      events.onSessionEnded?.();
    });

    connection.start()
      .then(() => connection.invoke("JoinSession", sessionId))
      .catch(() => { /* connection failed, REST fallback */ });

    return () => {
      connection.invoke("LeaveSession", sessionId).catch(() => {});
      connection.stop();
    };
  }, [sessionId, accessToken]);

  return connectionRef;
}
