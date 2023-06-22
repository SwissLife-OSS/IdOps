# syntax=docker/dockerfile:1.4
FROM --platform=$BUILDPLATFORM node:14.4.0-alpine AS development

RUN mkdir /workspace
WORKDIR /workspace
COPY package.json ./
COPY yarn.lock ./

RUN yarn install

CMD ["yarn", "run", "serve"]