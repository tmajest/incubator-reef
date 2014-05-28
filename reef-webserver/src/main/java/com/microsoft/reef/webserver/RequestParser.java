/**
 * Copyright (C) 2014 Microsoft Corporation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *         http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.microsoft.reef.webserver;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.util.*;

/**
 * Parse HttpServletRequest
 */
public class RequestParser {
    private final HttpServletRequest request;
    private final String pathInfo;
    private final String method;
    private final String queryString;
    private final String requestUri;
    private final String requestUrl;
    private final Map<String, String> headers = new HashMap();
    private final byte[] inputStream;
    private final String targetSpecification;
    private final String targetEntity;
    private final Map<String, List<String>> queryPairs = new LinkedHashMap<String, List<String>>();

    /**
     * parse HttpServletRequest
     *
     * @param request
     * @throws IOException
     * @throws ServletException
     */
    public RequestParser(final HttpServletRequest request) throws IOException, ServletException {
        this.request = request;

        pathInfo = request.getPathInfo();
        method = request.getMethod();
        requestUri = request.getRequestURI();
        requestUrl = request.getRequestURL().toString();

        if (request.getQueryString() != null ) {
            queryString = request.getQueryString();
        } else {
            queryString = "";
        }

        final Enumeration hn = request.getHeaderNames();
        while (hn.hasMoreElements()) {
            final String s = (String) hn.nextElement();
            final String header = request.getHeader(s);
            headers.put(s, header);
        }

        final int len = request.getContentLength();
        if (len > 0) {
            inputStream = new byte[len];
            request.getInputStream().read(inputStream);
        } else {
            inputStream = new byte[0];
        }

        if (requestUri != null) {
            final String[] parts = requestUri.split("/");

            if (parts != null && parts.length > 1) {
                targetSpecification = parts[1];
            } else {
                targetSpecification = null;
            }

            if (parts != null && parts.length > 2) {
                targetEntity = parts[2];
            } else {
                targetEntity = null;
            }
        } else {
            targetSpecification = null;
            targetEntity = null;
        }

        if (queryString != null) {
            final String[] pairs = queryString.split("&");
            for (final String pair : pairs) {
                final int idx = pair.indexOf("=");
                if (idx != -1) {
                    final String rKey = pair.substring(0, idx);
                    final String rValue = pair.substring(idx + 1);
                    if(rKey != null && rValue != null) {
                        final String key = URLDecoder.decode(rKey, "UTF-8");
                        final String value = URLDecoder.decode(rValue, "UTF-8");
                        List<String> valuesList = queryPairs.get(key);
                        if (valuesList == null) {
                            valuesList = new ArrayList<String>(1);
                            queryPairs.put(key, valuesList);
                        }
                        valuesList.add(value);
                    }
                }
            }
        }
    }

    /**
     * get target to match specification like "Reef"
     *
     * @return specification
     */
    public String getTargetSpecification() {
        return targetSpecification;
    }

    /**
     * get query string like "id=12345"
     *
     * @return
     */
    public String getQueryString() {
        return queryString;
    }

    /**
     * get target target entity like "Evaluators"
     *
     * @return
     */
    public String getTargetEntity() {
        return targetEntity;
    }

    /**
     * get http request method like "Get"
     *
     * @return
     */
    public String getMethod() {
        return method;
    }

    /**
     * get input Stream
     *
     * @return
     */
    public byte[] getInputStream() {
        return inputStream;
    }

    /**
     * get request headers
     *
     * @return
     */
    public Map<String, String> getHeaders() {
        return headers;
    }

    /**
     * get query maps
     * @return
     */
    public Map<String, List<String>> getQueryMap() {
        return queryPairs;
    }

    /**
     * get request URL
     * @return
     */
    public String getRequestUrl() {
        return requestUrl;
    }

    /**
     * get pathInfo
     * @return
     */
    public String getPathInfo() {
        return pathInfo;
    }
}