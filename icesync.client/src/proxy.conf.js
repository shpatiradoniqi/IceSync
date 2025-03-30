const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
    ? env.ASPNETCORE_URLS.split(';')[0]
    : 'https://localhost:5000';  // Zëvendëso me portin aktual të backend-it

const PROXY_CONFIG = [
  {
    context: ["/api"], // Përfshin të gjitha API-të që fillojnë me "/api"
    target: target,
    secure: false,
    changeOrigin: true,
    logLevel: "info"  // E ndryshova nga "debug" në "info" për më pak spam në console
  }
];

module.exports = PROXY_CONFIG;
