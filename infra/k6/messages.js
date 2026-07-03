import http from "k6/http";
import { check } from "k6";

export const options = {
  scenarios: {
    messages: {
      executor: "constant-arrival-rate",
      rate: Number(__ENV.RATE || 50),
      timeUnit: "1s",
      duration: __ENV.DURATION || "30s",
      preAllocatedVUs: Number(__ENV.VUS || 20),
      maxVUs: Number(__ENV.MAX_VUS || 100),
    },
  },
  thresholds: {
    http_req_failed: ["rate<0.01"],
    http_req_duration: ["p(95)<500", "p(99)<1000"],
  },
};

const apiUrl = __ENV.API_URL || "http://localhost:8080/messages";

export default function () {
  const response = http.post(apiUrl);

  check(response, {
    "accepted": (result) => result.status === 202,
  });
}
