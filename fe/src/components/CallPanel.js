import { useEffect, useRef, useState } from "react";

const ICE_SERVERS = [
  { urls: "stun:stun.l.google.com:19302" },
  { urls: "stun:stun1.l.google.com:19302" }
];

export default function CallPanel({ hub, conversationId }) {
  const pcRef = useRef(null);
  const localRef = useRef(null);
  const remoteRef = useRef(null);
  const localStreamRef = useRef(null);

  const [inCall, setInCall] = useState(false);
  const [ringing, setRinging] = useState(false);
  const [incomingOffer, setIncomingOffer] = useState(null);

  function ensurePc() {
    if (pcRef.current) return pcRef.current;

    const pc = new RTCPeerConnection({ iceServers: ICE_SERVERS });

    pc.onicecandidate = (e) => {
      if (e.candidate) {
        hub.invoke("SendIceCandidate", conversationId, e.candidate);
      }
    };

    pc.ontrack = (e) => {
      const [stream] = e.streams;
      if (remoteRef.current) remoteRef.current.srcObject = stream;
    };

    pcRef.current = pc;
    return pc;
  }

  async function getLocalMedia({ video }) {
    const stream = await navigator.mediaDevices.getUserMedia({
      audio: true,
      video: !!video
    });
    localStreamRef.current = stream;
    if (localRef.current) localRef.current.srcObject = stream;
    return stream;
  }

  async function attachTracks(pc, stream) {
    stream.getTracks().forEach((t) => pc.addTrack(t, stream));
  }

  async function startCall({ video }) {
    setRinging(false);
    setIncomingOffer(null);

    const pc = ensurePc();
    const stream = await getLocalMedia({ video });
    await attachTracks(pc, stream);

    const offer = await pc.createOffer();
    await pc.setLocalDescription(offer);

    await hub.invoke("SendOffer", conversationId, offer);
    setInCall(true);
  }

  async function acceptCall() {
    if (!incomingOffer) return;

    const pc = ensurePc();
    // audio call default (bạn muốn video thì đổi true)
    const stream = await getLocalMedia({ video: false });
    await attachTracks(pc, stream);

    await pc.setRemoteDescription(incomingOffer);
    const answer = await pc.createAnswer();
    await pc.setLocalDescription(answer);

    await hub.invoke("SendAnswer", conversationId, answer);
    setInCall(true);
    setRinging(false);
    setIncomingOffer(null);
  }

  function rejectCall() {
    setRinging(false);
    setIncomingOffer(null);
  }

  function endCall() {
    setInCall(false);
    setRinging(false);
    setIncomingOffer(null);

    try {
      pcRef.current?.getSenders()?.forEach(s => s.track?.stop());
      pcRef.current?.close();
    } catch {}

    pcRef.current = null;

    if (localStreamRef.current) {
      localStreamRef.current.getTracks().forEach(t => t.stop());
      localStreamRef.current = null;
    }

    if (localRef.current) localRef.current.srcObject = null;
    if (remoteRef.current) remoteRef.current.srcObject = null;
  }

  useEffect(() => {
    if (!hub || !conversationId) return;

    const onOffer = async (payload) => {
      // offer từ chính mình gửi qua group cũng quay về -> ignore
      // payload.fromUserId so với token decode thì hơi dài; quick: nếu inCall đang true thì ignore offer mới
      if (inCall) return;

      setRinging(true);
      setIncomingOffer(payload.offer);
    };

    const onAnswer = async (payload) => {
      const pc = ensurePc();
      await pc.setRemoteDescription(payload.answer);
    };

    const onIce = async (payload) => {
      const pc = ensurePc();
      try {
        await pc.addIceCandidate(payload.candidate);
      } catch {}
    };

    hub.on("call:offer", onOffer);
    hub.on("call:answer", onAnswer);
    hub.on("call:ice", onIce);

    return () => {
      hub.off("call:offer", onOffer);
      hub.off("call:answer", onAnswer);
      hub.off("call:ice", onIce);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hub, conversationId, inCall]);

  return (
    <div className="pcard" style={{ marginTop: 12 }}>
      <div style={{ display: "flex", gap: 8, alignItems: "center" }}>
        <div style={{ fontWeight: 900 }}>Call</div>
        <div style={{ flex: 1 }} />
        {!inCall && (
          <>
            <button className="pcard-action" onClick={() => startCall({ video: false })}>
              📞 Audio
            </button>
            <button className="pcard-action" onClick={() => startCall({ video: true })}>
              🎥 Video
            </button>
          </>
        )}
        {inCall && (
          <button className="pcard-action" onClick={endCall} style={{ color: "#dc2626" }}>
            ⛔ End
          </button>
        )}
      </div>

      {ringing && (
        <div style={{ marginTop: 10, display: "flex", gap: 8, alignItems: "center" }}>
          <div style={{ fontWeight: 800 }}>Incoming call…</div>
          <div style={{ flex: 1 }} />
          <button className="pcard-action" onClick={acceptCall}>✅ Accept</button>
          <button className="pcard-action" onClick={rejectCall}>❌ Reject</button>
        </div>
      )}

      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 10, marginTop: 10 }}>
        <div>
          <div style={{ fontSize: 12, color: "#64748b" }}>You</div>
          <video ref={localRef} autoPlay playsInline muted style={{ width: "100%", borderRadius: 12, background:"#0f172a" }} />
        </div>
        <div>
          <div style={{ fontSize: 12, color: "#64748b" }}>Other</div>
          <video ref={remoteRef} autoPlay playsInline style={{ width: "100%", borderRadius: 12, background:"#0f172a" }} />
        </div>
      </div>
    </div>
  );
}
